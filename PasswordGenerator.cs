using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PasswordGenerator
{
    public class PasswordGenerator
    {
        private const string SimpleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string NumberChars = "0123456789";
        private const string SymbolChars = "!@#$%^&*()";

        private readonly int length;
        private readonly Method method;

        public PasswordGenerator()
        {
            this.length = 8;
            this.method = Method.Strong;
        }

        public PasswordGenerator(int length, Method method)
        {
            this.length = length;
            this.method = method;
        }

        public string Generate()
        {
            switch (method)
            {
                case Method.Simple:
                    return GetUniqueKey(SimpleChars, length).ToString();
                case Method.Strong:
                    return GetUniqueKeyStrong();
                case Method.Special:
                    return GetUniqueKeyStrongWithSymbols();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetUniqueKeyStrong()
        {
            var lengthForNumbers = length / 3;
            var numberChars = GetUniqueKey(NumberChars, lengthForNumbers);

            var simpleChars = GetUniqueKey(SimpleChars, length - lengthForNumbers);

            var sum = numberChars.Append(simpleChars);
            var scrambled = Scramble(sum.ToString());

            return new string(scrambled.ToArray());
        }

        private string GetUniqueKeyStrongWithSymbols()
        {
            var lengthForNumbers = length / 4;
            var numberChars = GetUniqueKey(NumberChars, lengthForNumbers);

            var lengthForSpec = length / 4;
            var specChars = GetUniqueKey(SymbolChars, lengthForNumbers);

            var simpleChars = GetUniqueKey(SimpleChars, length - lengthForNumbers - lengthForSpec);
            var sum = numberChars.Append(specChars).Append(simpleChars);
            var scrambled = Scramble(sum.ToString());

            return new string(scrambled.ToArray());
        }

        private List<char> Scramble(string s)
        {
            var chars = s.ToList().OrderBy(x => Guid.NewGuid()).ToList();

            while (!SimpleChars.Contains(chars.First()))
            {
                chars = Scramble(new string(chars.ToArray()));
            }

            return chars;
        }

        private static StringBuilder GetUniqueKey(string symbols, int size)
        {
            var chars = symbols.ToCharArray();

            var data = new byte[4 * size];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }

            var result = new StringBuilder(size);
            for (var i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result;
        }
    }

    public enum Method
    {
        Simple,
        Strong,
        Special
    }
}