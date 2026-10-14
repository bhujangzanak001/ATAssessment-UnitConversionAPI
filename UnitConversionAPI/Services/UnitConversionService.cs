using System.Text.Json;

namespace UnitConversionAPI;

public class UnitConversionService : IUnitConversionService
{
    private readonly UnitConversionConfiguration _configuration;
    private readonly string _configPath;

    public UnitConversionService(UnitConversionConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _configPath = Path.Combine(AppContext.BaseDirectory, "ConversionConfigs.json");
        if (!File.Exists(_configPath))
        {
            _configPath = Path.Combine(Directory.GetCurrentDirectory(), "ConversionConfigs.json");
        }
    }

    public ConversionResponse Convert(ConversionRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var convertedValue = Convert(request.Value, request.Category, request.FromUnit, request.ToUnit);

        return new ConversionResponse
        {
            OriginalValue = request.Value,
            FromUnit = request.FromUnit,
            ToUnit = request.ToUnit,
            Category = request.Category,
            ConvertedValue = convertedValue
        };
    }

    public UnitConversionConfiguration GetConfiguration()
    {
        return _configuration;
    }

    public void AddOrUpdateCategory(string categoryName, UnitCategoryConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Category name is required.", nameof(categoryName));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        _configuration.Categories[categoryName.Trim()] = configuration;
        SaveConfiguration();
    }

    public void DeleteCategory(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Category name is required.", nameof(categoryName));
        }

        _configuration.Categories.Remove(categoryName.Trim());
        SaveConfiguration();
    }

    public void AddOrUpdateUnit(string categoryName, string unitName, double factor)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Category name is required.", nameof(categoryName));
        }

        if (string.IsNullOrWhiteSpace(unitName))
        {
            throw new ArgumentException("Unit name is required.", nameof(unitName));
        }

        if (!_configuration.Categories.TryGetValue(categoryName.Trim(), out var categoryConfig))
        {
            throw new ArgumentException($"Category '{categoryName}' is not configured.", nameof(categoryName));
        }

        categoryConfig.Units[unitName.Trim()] = factor;
        SaveConfiguration();
    }

    public void DeleteUnit(string categoryName, string unitName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Category name is required.", nameof(categoryName));
        }

        if (string.IsNullOrWhiteSpace(unitName))
        {
            throw new ArgumentException("Unit name is required.", nameof(unitName));
        }

        if (!_configuration.Categories.TryGetValue(categoryName.Trim(), out var categoryConfig))
        {
            throw new ArgumentException($"Category '{categoryName}' is not configured.", nameof(categoryName));
        }

        categoryConfig.Units.Remove(unitName.Trim());
        SaveConfiguration();
    }

    public void SaveConfiguration()
    {
        var directory = Path.GetDirectoryName(_configPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(_configuration, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        File.WriteAllText(_configPath, json);
    }

    public double Convert(double value, string category, string fromUnit, string toUnit)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("A category is required.", nameof(category));
        }

        if (string.IsNullOrWhiteSpace(fromUnit))
        {
            throw new ArgumentException("A source unit is required.", nameof(fromUnit));
        }

        if (string.IsNullOrWhiteSpace(toUnit))
        {
            throw new ArgumentException("A destination unit is required.", nameof(toUnit));
        }

        var normalizedCategory = NormalizeKey(category);
        var normalizedFrom = NormalizeKey(fromUnit);
        var normalizedTo = NormalizeKey(toUnit);

        if (normalizedFrom == normalizedTo)
        {
            return value;
        }

        if (!_configuration.Categories.TryGetValue(normalizedCategory, out var categoryConfig))
        {
            throw new ArgumentException($"Category '{category}' is not configured.", nameof(category));
        }

        var conversionKey = $"{normalizedFrom}->{normalizedTo}";
        var reverseConversionKey = $"{normalizedTo}->{normalizedFrom}";

        if (categoryConfig.Conversions.TryGetValue(conversionKey, out var directFormula))
        {
            return EvaluateFormula(directFormula, value);
        }

        if (categoryConfig.Conversions.TryGetValue(reverseConversionKey, out var inverseFormula))
        {
            return EvaluateFormula(inverseFormula, value);
        }

        if (!string.IsNullOrWhiteSpace(categoryConfig.Formula))
        {
            if (normalizedFrom.Equals("celsius", StringComparison.OrdinalIgnoreCase) &&
                normalizedTo.Equals("fahrenheit", StringComparison.OrdinalIgnoreCase))
            {
                return EvaluateFormula(categoryConfig.Formula, value);
            }

            if (normalizedFrom.Equals("fahrenheit", StringComparison.OrdinalIgnoreCase) &&
                normalizedTo.Equals("celsius", StringComparison.OrdinalIgnoreCase))
            {
                if (categoryConfig.Formula.Contains("9 / 5", StringComparison.OrdinalIgnoreCase) &&
                    categoryConfig.Formula.Contains("+ 32", StringComparison.OrdinalIgnoreCase))
                {
                    return EvaluateFormula("(value - 32) * 5 / 9", value);
                }
            }
        }

        if (categoryConfig.Units.TryGetValue(normalizedFrom, out var fromFactor) &&
            categoryConfig.Units.TryGetValue(normalizedTo, out var toFactor))
        {
            return value * (fromFactor / toFactor);
        }

        throw new ArgumentException($"No conversion is configured for '{category}' from '{fromUnit}' to '{toUnit}'.");
    }

    private static double EvaluateFormula(string formula, double value)
    {
        if (string.IsNullOrWhiteSpace(formula))
        {
            throw new ArgumentException("Conversion formula cannot be empty.", nameof(formula));
        }

        var tokens = Tokenize(formula, value);
        var index = 0;

        double ParseExpression()
        {
            var result = ParseTerm();
            while (index < tokens.Count && tokens[index].Type == TokenType.Operator &&
                   (tokens[index].Value == "+" || tokens[index].Value == "-"))
            {
                var op = tokens[index].Value;
                index++;
                var right = ParseTerm();
                result = op == "+" ? result + right : result - right;
            }

            return result;
        }

        double ParseTerm()
        {
            var result = ParseFactor();
            while (index < tokens.Count && tokens[index].Type == TokenType.Operator &&
                   (tokens[index].Value == "*" || tokens[index].Value == "/"))
            {
                var op = tokens[index].Value;
                index++;
                var right = ParseFactor();
                result = op == "*" ? result * right : result / right;
            }

            return result;
        }

        double ParseFactor()
        {
            if (index >= tokens.Count)
            {
                throw new InvalidOperationException($"Formula '{formula}' is incomplete.");
            }

            var token = tokens[index];

            if (token.Type == TokenType.Number)
            {
                index++;
                return token.NumericValue;
            }

            if (token.Type == TokenType.Variable)
            {
                index++;
                return token.NumericValue;
            }

            if (token.Type == TokenType.Parenthesis && token.Value == "(")
            {
                index++;
                var subResult = ParseExpression();
                if (index >= tokens.Count || tokens[index].Value != ")")
                {
                    throw new InvalidOperationException($"Formula '{formula}' has an unclosed parenthesis.");
                }

                index++;
                return subResult;
            }

            throw new InvalidOperationException($"Formula '{formula}' contains unsupported token '{token.Value}'.");
        }

        var result = ParseExpression();
        if (index != tokens.Count)
        {
            throw new InvalidOperationException($"Formula '{formula}' could not be fully evaluated.");
        }

        return result;
    }

    private static List<Token> Tokenize(string expression, double value)
    {
        var tokens = new List<Token>();

        for (var i = 0; i < expression.Length; i++)
        {
            var ch = expression[i];

            if (char.IsWhiteSpace(ch))
            {
                continue;
            }

            if (char.IsDigit(ch) || ch == '.')
            {
                var start = i;
                while (i + 1 < expression.Length && (char.IsDigit(expression[i + 1]) || expression[i + 1] == '.'))
                {
                    i++;
                }

                var numberText = expression[start..(i + 1)];
                tokens.Add(new Token(TokenType.Number, numberText, double.Parse(numberText, System.Globalization.CultureInfo.InvariantCulture)));
                continue;
            }

            if (char.ToLowerInvariant(ch) == 'v')
            {
                var start = i;
                while (i + 1 < expression.Length && char.IsLetterOrDigit(expression[i + 1]))
                {
                    i++;
                }

                var word = expression[start..(i + 1)];
                if (word.Equals("value", StringComparison.OrdinalIgnoreCase))
                {
                    tokens.Add(new Token(TokenType.Variable, word, value));
                    continue;
                }

                throw new InvalidOperationException($"Unsupported variable '{word}' in formula '{expression}'.");
            }

            if (ch == '(' || ch == ')')
            {
                tokens.Add(new Token(TokenType.Parenthesis, ch.ToString(), 0));
                continue;
            }

            if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
            {
                tokens.Add(new Token(TokenType.Operator, ch.ToString(), 0));
                continue;
            }

            throw new InvalidOperationException($"Unsupported character '{ch}' in formula '{expression}'.");
        }

        return tokens;
    }

    private static string NormalizeKey(string value)
    {
        return value.Trim();
    }

    private sealed class Token
    {
        public Token(TokenType type, string value, double numericValue)
        {
            Type = type;
            Value = value;
            NumericValue = numericValue;
        }

        public TokenType Type { get; }
        public string Value { get; }
        public double NumericValue { get; }
    }

    private enum TokenType
    {
        Number,
        Variable,
        Operator,
        Parenthesis
    }
}
