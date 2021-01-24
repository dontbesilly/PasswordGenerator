using System.Collections.Generic;
using Xunit;

namespace PasswordGenerator.Tests
{
    public class PasswordGenerate
    {
        [Fact]
        public void DefaultPassword()
        {
            var pg = new PasswordGenerator();
            var password = pg.Generate();

            Assert.Equal(password.Length, PasswordGenerator.DefaultLength);
            Assert.Contains(password, char.IsDigit);
        }

        [Fact]
        public void PasswordLength()
        {
            const int length = 20;
            var pg = new PasswordGenerator(length);
            var password = pg.Generate();

            Assert.Equal(password.Length, length);
        }

        [Fact]
        public void LoadTest()
        {
            const int length = 100;
            var pg = new PasswordGenerator(length, Method.Special);

            var list = new List<string>();
            const int count = 100;
            for (int i = 0; i < count; i++)
            {
                var password = pg.Generate();
                list.Add(password);
            }

            Assert.Equal(100, list.Count);
        }
    }
}