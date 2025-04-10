using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace BlackBoxTests.Utils
{
    public class ElementActions
    {
        private readonly ChromeDriver _driver;
        private readonly string _searchId = "dt-search-0";

        public ElementActions(ChromeDriver driver)
        {
            _driver = driver;
        }

        public IWebElement CheckRowContaining(string text)
        {
            var row = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(text));
            Assert.That(row, Is.Not.Null, $"Row with '{text}' not found in the list.");

            return row;
        }

        public void CheckRowDoesNotExist(string text)
        {
            var row = _driver.FindElements(By.CssSelector("table tbody tr")).FirstOrDefault(row => row.Text.Contains(text));
            Assert.That(row, Is.Null, $"Row with '{text}' found in the list!");
        }

        /// <summary>
        /// Fills in a date field.
        /// </summary>
        /// <param name="id">Id of date field</param>
        /// <param name="date">Date in the form: MMDDYYYY</param>
        /// <param name="time">Time in the form: HH:MM{AM|PM}</param>
        /// <returns>Date field element</returns>
        public IWebElement FillDateField(string id, string date, string time)
        {
            var dateTimeField = _driver.FindElement(By.Id(id));
            dateTimeField.Clear();
            dateTimeField.SendKeys(date);
            dateTimeField.SendKeys(Keys.Tab);
            dateTimeField.SendKeys(time);

            return dateTimeField;
        }

        public void VerifyDateFieldValue(string id, string date)
        {
            var currentDate = _driver.FindElement(By.Id(id)).GetAttribute("value")!.Trim();
            Assert.That(currentDate, Is.EqualTo(date), "Date mismatch.");
        }

        /// <summary>
        /// Selects a dropdown by value
        /// </summary>
        /// <param name="id">Id of dropdown</param>
        /// <param name="dropdownValue">Value to select</param>
        /// <returns>Select element</returns>
        public SelectElement SelectDropdownItem(string id, string dropdownValue)
        {
            var dropdown = _driver.FindElement(By.Id(id));
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByValue(dropdownValue);

            return selectElement;
        }

        public void VerifyDropdownItem(string id, string dropdownValue, string message)
        {
            var dropdown = _driver.FindElement(By.Id(id));
            var selectElement = new SelectElement(dropdown);
            var currentValue = selectElement.SelectedOption.Text.Trim();
            Assert.That(currentValue, Is.EqualTo(dropdownValue), message);
        }

        /// <summary>
        /// Clears the text field and fills it with the specified text.
        /// </summary>
        /// <param name="id">Id of text field</param>
        /// <param name="text">Text to fill</param>
        /// <returns>Text field</returns>
        public IWebElement FillTextField(string id, string text)
        {
            var textField = _driver.FindElement(By.Id(id));
            textField.Clear();
            textField.SendKeys(text);

            return textField;
        }

        public void VerifyTextField(string id, string text, string message)
        {
            var currentValue = _driver.FindElement(By.Id(id)).GetAttribute("value")!.Trim();
            Assert.That(currentValue, Is.EqualTo(text), message);
        }

        public void SearchByValue(string value)
        {
            _driver.FindElement(By.Id(_searchId)).SendKeys(value);
        }
    }
}
