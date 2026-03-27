namespace Genzy.Base.DTO;

/// <summary>
/// DTO dùng chung cho danh sách lựa chọn (dropdown/options): id và name.
/// </summary>
public class OptionDto
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
