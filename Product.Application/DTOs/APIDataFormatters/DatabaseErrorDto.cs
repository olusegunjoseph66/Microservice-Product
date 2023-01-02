namespace Product.Application.DTOs.APIDataFormatters
{
    public class DatabaseErrorDto
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string MaskedColumnValue { get; set; }
    }
}
