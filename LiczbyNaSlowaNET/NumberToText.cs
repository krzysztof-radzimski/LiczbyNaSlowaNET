﻿
// Copyright (c) 2014 Przemek Walkowski

namespace LiczbyNaSlowaNET
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Algorithms;
    using Dictionaries;
    using Dictionaries.Currencies;

    internal enum Phase { BeforeComma = 1, AfterComma };

    public enum Currency
    {
        NONE,
        PLN,
        CHF,
        CZK,
        EUR,
        HUF,
        JPY,
        LTL,
        NOK,
        SEK,
        USD
    }

    public static class NumberToText
    {
        public static List<ICurrencyDeflation> ListCurrencyDeflation { get; set; }

        /// <summary>
        /// Convert (int) number into words.
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <param name="currency">Currency of number</param>
        /// <returns>he words describe number</returns>
        public static string Convert(int number, Currency currency = Currency.NONE)
        {
            var options = new NumberToTextOptions();

            ConvertToICurrenctyDeflation(currency, options);

            return CommonConvert(new[] { number }, options);
        }

        public static string Convert(decimal number, Currency currency = Currency.NONE)
        {
            var options = new NumberToTextOptions();

            ConvertToICurrenctyDeflation(currency, options);

            return CommonConvert(PrepareNumbers(number), options);
        }

        public static string Convert(int number, NumberToTextOptions options)
        {
            return CommonConvert(new[] { number }, options);
        }

        public static string Convert(decimal number, NumberToTextOptions options)
        {
            return CommonConvert(PrepareNumbers(number), options);
        }
        
        private static string CommonConvert(IEnumerable<int> numbers, NumberToTextOptions options)
        {
            var dictionaries = options.Dictionary ?? new PolishDictionary();
                               
            var algorithm = new CurrencyAlgorithm(dictionaries)
            {
                Numbers = numbers,
                Options = options
            };

            return algorithm.Build();
        }

        private static IEnumerable<int> PrepareNumbers(decimal numbers)
       {
            var splitNumber = Math.Round(numbers, 2).ToString(CultureInfo.InvariantCulture).Replace('.', '@').Replace(',', '@').Split('@');

            if (splitNumber.Length > 1 && splitNumber[1].Length == 1)
            {
                splitNumber[1] += "0";
            }
            
            var allNumbers = new List<int>();

            foreach (var t in splitNumber)
            {
                int intNumber;

                if (int.TryParse(t, out intNumber))
                {
                    allNumbers.Add(intNumber);
                }
            }

            return allNumbers;
        }

        private static void ConvertToICurrenctyDeflation(Currency currency, INumberToTextOptions options)
        {
            options.Currency = 
                new CurrencyDeflationFactory(false).CreateInstance(currency.ToString());
        }
    }
}
