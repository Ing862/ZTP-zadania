using System;
using System.Collections.Generic;
using System.Text;

abstract class Cell
{
	public abstract string GetValue();
	public override string ToString()
	{
		return GetValue().PadRight(15);
	}
}

class TextCell : Cell
{
	private string value;

	public TextCell(string value)
	{
		this.value = value;
	}

	public override string GetValue()
	{
		return value;
	}
}

class NumberCell : Cell
{
	private int value;

	public NumberCell(int value)
	{
		this.value = value;
	}

	public override string GetValue()
	{
		return value.ToString();
	}
}

class BooleanCell : Cell
{
	private bool value;

	public BooleanCell(bool value)
	{
		this.value = value;
	}

	public override string GetValue()
	{
		return value ? "True" : "False";
	}
}



abstract class Header
{
	protected string Name { get; set; }
	public abstract Cell CreateCell(object value);
	public abstract Cell CreateDefaultCell();
	public override string ToString()
	{
		return Name;
	}
}

class TextHeader : Header
{
	public TextHeader(string name)
	{
		Name = name;
	}

	public override Cell CreateCell(object value)
	{
		return new TextCell(value?.ToString() ?? "");
	}

	public override Cell CreateDefaultCell()
	{
		return new TextCell("");
	}

}

class NumberHeader : Header
{
	private int Value { get; set; }

	public NumberHeader(string name)
	{
		Name = name;
	}

	public override Cell CreateCell(object value)
	{
		return new NumberCell(Convert.ToInt32(value));
	}

	public override Cell CreateDefaultCell()
	{
		return new NumberCell(0);
	}

}

class BooleanHeader : Header
{
	private bool Value { get; set; }
	public BooleanHeader(string name)
	{
		Name = name;
	}

	public override Cell CreateCell(object value)
	{
		return new BooleanCell(Convert.ToBoolean(value));
	}

	public override Cell CreateDefaultCell()
	{
		return new BooleanCell(false);
	}

}

// Klasa reprezentująca tabelę
class Table
{
	private readonly List<Header> headers;  // Lista nagłówków kolumn
	private readonly List<List<Cell>> rows; // Lista wierszy (każdy wiersz to lista komórek)

	public Table()
	{
		headers = new List<Header>();
		rows = new List<List<Cell>>();
	}

	public void AddColumn(Header header)
	{
		headers.Add(header);

		// Dodajemy puste komórki do każdego z istniejących wierszy
		foreach (var row in rows)
		{
			row.Add(header.CreateDefaultCell());
		}
	}

	public void AddRow(params object[] cells)
	{
		if (cells.Length != headers.Count)
		{
			//Zakomentowałem obsługę błędu ponieważ przy braku danych powinno zostać stworzony pusty Cell w Headeerze
			//throw new ArgumentException("Liczba wartości nie zgadza się z liczbą kolumn.");
		}

		// Dodajemy wiersz wypełniony komórkami z wartością
		var newRow = new List<Cell>();
		for (int i = 0; i < cells.Length; i++)
		{
			newRow.Add(headers[i].CreateCell(cells[i]));
		}
		rows.Add(newRow);
	}

	public override string ToString()
	{
		var sb = new StringBuilder();

		// Dodajemy nagłówki
		foreach (var header in headers)
		{
			sb.Append(header.ToString().PadRight(15));
		}
		sb.AppendLine();

		// Dodajemy separator
		sb.AppendLine(new string('-', headers.Count * 15));

		// Dodajemy wiersze
		foreach (var row in rows)
		{
			foreach (var cell in row)
			{
				sb.Append(cell.ToString());
			}
			sb.AppendLine();
		}

		return sb.ToString();
	}
}

class Program
{
	static void Main(string[] args)
	{
		// Tworzymy nową tabelę
		Table table = new Table();

		// Dodajemy kolumny
		table.AddColumn(new NumberHeader("ID"));
		table.AddColumn(new TextHeader("Name"));
		table.AddColumn(new NumberHeader("Age"));
		table.AddColumn(new BooleanHeader("Is Student"));

		// Dodajemy wiersze
		table.AddRow(1, "Alice", 30);
		table.AddRow(2, "Bob", 25, true);
		table.AddRow(3, "Charlie", 35, false);
		table.AddRow();
		table.AddRow(5);


		// Wyświetlamy tabelę
		Console.WriteLine(table.ToString());

		Console.ReadKey();
	}
}