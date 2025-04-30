using System.Text.Json;

namespace ReanimHelper;

class Program
{
	static void Main(string[] args)
	{
		Reanim reanim = Utils.LoadFromCompiled(args[0]);
		string file = Path.GetFileName(args[0]).Split('.')[0];
		string reanimStr = JsonSerializer.Serialize<Reanim>(reanim);
		using StreamWriter writer = new StreamWriter($"{file}.json");
		writer.Write(reanimStr);
	}
}
