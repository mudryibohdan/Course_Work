using System.Globalization;

namespace Wallet.ConsoleApp;

internal static class ConsoleInput
{
    public static string ReadRequiredString(string prompt, int maxLength = 200)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Значення не може бути порожнім. Спробуйте ще раз.");
                continue;
            }

            if (input.Length > maxLength)
            {
                Console.WriteLine($"Значення занадто довге. Максимальна довжина: {maxLength} символів.");
                continue;
            }

            return input;
        }
    }

    public static string? ReadOptionalString(string prompt, int maxLength = 500)
    {
        while (true)
        {
            Console.Write($"{prompt} (залиште порожнім, якщо не потрібно): ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            input = input.Trim();
            if (input.Length > maxLength)
            {
                Console.WriteLine($"Значення занадто довге. Максимальна довжина: {maxLength} символів.");
                continue;
            }

            return input;
        }
    }

    public static decimal ReadDecimal(string prompt, decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var raw = Console.ReadLine();
            if (TryParseDecimal(raw, out var value))
            {
                if (value < minValue)
                {
                    Console.WriteLine($"Значення має бути не меншим за {minValue}.");
                    continue;
                }

                if (value > maxValue)
                {
                    Console.WriteLine($"Значення має бути не більшим за {maxValue}.");
                    continue;
                }

                return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
            }

            Console.WriteLine("Не вдалося розпізнати число. Використовуйте формат 1234.56 або 1234,56.");
        }
    }

    public static int ReadInt(string prompt, int minValue, int maxValue)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var value))
            {
                if (value < minValue || value > maxValue)
                {
                    Console.WriteLine($"Введіть число від {minValue} до {maxValue}.");
                    continue;
                }

                return value;
            }

            Console.WriteLine("Не вдалося розпізнати ціле число.");
        }
    }

    public static Guid ReadGuid(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            var input = Console.ReadLine();
            if (Guid.TryParse(input, out var value))
            {
                return value;
            }

            Console.WriteLine("Невірний формат GUID. Спробуйте ще раз.");
        }
    }

    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} (формат YYYY-MM-DD): ");
            var input = Console.ReadLine();
            if (TryParseDate(input, out var date))
            {
                return date;
            }

            Console.WriteLine("Невірний формат дати.");
        }
    }

    public static (decimal? min, decimal? max) ReadAmountRange()
    {
        Console.Write("Мінімальна сума (залиште порожнім, якщо не потрібно): ");
        var minRaw = Console.ReadLine();
        decimal? min = null;
        if (TryParseDecimal(minRaw, out var minParsed))
        {
            min = minParsed;
        }

        Console.Write("Максимальна сума (залиште порожнім, якщо не потрібно): ");
        var maxRaw = Console.ReadLine();
        decimal? max = null;
        if (TryParseDecimal(maxRaw, out var maxParsed))
        {
            max = maxParsed;
        }

        if (min.HasValue && max.HasValue && max < min)
        {
            Console.WriteLine("Максимальне значення не може бути меншим за мінімальне. Діапазон буде проігнорований.");
            return (null, null);
        }

        return (min, max);
    }

    private static bool TryParseDecimal(string? input, out decimal value)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            value = default;
            return false;
        }

        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value)
            || decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    private static bool TryParseDate(string? input, out DateTime dateTime)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            dateTime = default;
            return false;
        }

        if (DateOnly.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            dateTime = date.ToDateTime(TimeOnly.MinValue);
            return true;
        }

        if (DateOnly.TryParse(input, out date))
        {
            dateTime = date.ToDateTime(TimeOnly.MinValue);
            return true;
        }

        dateTime = default;
        return false;
    }
}

