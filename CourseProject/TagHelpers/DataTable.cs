using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Reflection;

namespace CourseProject.TagHelpers
{
    [HtmlTargetElement("data-table")]
    public class DataTable : TagHelper
    {
        public string Id { get; set; } = string.Empty;
        public ModelExpression Items { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "table";
            output.Attributes.SetAttribute("class", "table table-striped table-bordered datatable");

            if (!string.IsNullOrWhiteSpace(Id))
            {
                output.Attributes.SetAttribute("id", Id);
            }

            // Check if the model is not a collection or if it's an empty collection.
            if (Items.Model is not IEnumerable<object> items || !items.Any())
            {
                output.Content.SetHtmlContent("<thead><tr><th>No data</th></tr></thead>");
                return;
            }

            var itemType = items.First().GetType();
            var properties = itemType.GetProperties();

            // Table Head
            var thead = "<thead><tr>" +
                        string.Join("", properties.Select(p => $"<th>{p.Name}</th>")) +
                        "</tr></thead>";

            // Table Body
            var tbody = "<tbody>";

            foreach (var item in items)
            {
                tbody += "<tr>";

                foreach (var prop in properties)
                {
                    var cellValue = GetCellValue(item, prop);
                    tbody += $"<td>{cellValue}</td>";
                }

                tbody += "</tr>";
            }

            tbody += "</tbody>";
            output.Content.SetHtmlContent(thead + tbody);
        }

        private static string GetCellValue(object item, PropertyInfo prop)
        {
            var value = prop.GetValue(item);

            if (value == null)
            {
                return "";
            }

            if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
            {
                var subProps = value.GetType()
                    // Include only string or value types (e.g., string, int, DateTime)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    // Exclude ID to avoid showing things like "1 John Smith"
                    .Where(sp => (sp.PropertyType == typeof(string) || sp.PropertyType.IsValueType) && sp.Name != "ID")
                    // Convert each sub-property to string
                    .Select(sp => sp.GetValue(value)?.ToString());

                // Join the values with space (e.g., "John Smith")
                return string.Join(" ", subProps).Trim();
            }

            return value.ToString() ?? "";
        }
    }
}
