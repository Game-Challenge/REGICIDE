//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

// Generate From PlayerFeatureConfig.xlsx

public class PlayerFeatureConfig
{
	public int ID; // 特性ID
	public string Name; // 特性名称
	public int SR; // 稀有度
	public int Start; // 开局可选
	public int CanUse; // 是否主动
	public int[] BuffIDArray; // BuffID数组
	public int UseColor; // 使用颜色
	public string ColorStr; // 颜色Str
	public float Rate; // 概率
	public int Slot; // 占用格子
	public string Desc; // 特性描述
}


// End of Auto Generated Code
