using System;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;
using System.Threading;

//singleton i object pool
class ConnectionManager
{
	private static ConnectionManager? instance;

	// kolejka połączeń
	private Queue<IDatabaseConnection> connections = new Queue<IDatabaseConnection>();
	// liczba połączeń
	private int ConnectionCount = 0;
	// maksymalna liczba dostępnych połączeń
	private const int MaxConnections = 3;

	//prywatny konstruktor menagera
	private ConnectionManager()
	{

	}

	public static ConnectionManager GetInstance()
	{
		if (instance == null)
		{
			instance = new ConnectionManager();
		}
		return instance;
	}

	public IDatabaseConnection GetConnection(Database database)
	{
		if (connections.Count < MaxConnections)
		{
			// Tworzenie nowego połączenia, jeśli nie osiągnięto jeszcze limitu
			IDatabaseConnection newConnection = new Database.DatabaseConnection(database);
			connections.Enqueue(newConnection); // Dodanie do kolejki

			Console.WriteLine($"Nowe połączenie utworzone");

			return newConnection;
		}
		else
		{
			// Recykling istniejącego połączenia
			IDatabaseConnection oldestConnection = connections.Dequeue();
			connections.Enqueue(oldestConnection); // Ustawienie z powrotem na koniec kolejki

			Console.WriteLine($"Użycie istniejącego połączenia");

			return oldestConnection;
		}
	}
}

interface IDatabaseConnection
{
	int AddRecord(string name, int age);

	void UpdateRecord(int id, string newName, int newAge);

	void DeleteRecord(int id);

	Record? GetRecord(int id);

	void ShowAllRecords();
}

// Prosty rekord w bazie danych
class Record
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Age { get; set; }

	public Record(int id, string name, int age)
	{
		Id = id;
		Name = name;
		Age = age;
	}

	public override string ToString()
	{
		return $"Record [ID={Id}, Name={Name}, Age={Age}]";
	}
}

// Prosta baza danych
// Multiton                                          !! DO KOREKTY !!
class Database
{
	//private List<Database> databases = new List<Database>(); //Lista kluczy baz danych
	private readonly List<Record> records; // Lista przechowująca rekordy
	private int nextId = 1; // Licznik do generowania unikalnych ID

	//instance bazy na podstawie nazwy

	// tworzy bazę danych i pustą listę rekordów
	public Database()
	{
		records = new List<Record>();
	}

	// Połaczenie z bazą danych
	public class DatabaseConnection : IDatabaseConnection
	{
		private readonly Database db;

		public DatabaseConnection(Database database)
		{
			db = database;
		}

		// Dodawanie nowego rekordu
		public int AddRecord(string name, int age)
		{
			Record newRecord = new(db.nextId++, name, age);
			db.records.Add(newRecord);
			Console.WriteLine($"Inserted: {newRecord}");
			return db.nextId - 1; // zwracamy id dodanego rekordu
		}

		// Pobieranie rekordu po ID
		public Record? GetRecord(int id)
		{
			return db.records.Where(rec => rec.Id == id).FirstOrDefault();
		}

		// Aktualizowanie rekordu po ID
		public void UpdateRecord(int id, string newName, int newAge)
		{
			Record? optionalRecord = GetRecord(id);

			if (optionalRecord != null)
			{
				Record record = optionalRecord;
				record.Name = newName;
				record.Age = newAge;
				Console.WriteLine($"Updated: {record}");
			}
			else
			{
				Console.WriteLine($"Record with ID {id} not found.");
			}
		}

		// Usuwanie rekordu po ID
		public void DeleteRecord(int id)
		{
			Record? optionalRecord = GetRecord(id);

			if (optionalRecord != null)
			{
				db.records.Remove(optionalRecord);
				Console.WriteLine($"Deleted record with ID {id}");
			}
			else
			{
				Console.WriteLine($"Record with ID {id} not found.");
			}
		}

		// Wyświetlanie wszystkich rekordów
		public void ShowAllRecords()
		{
			if (db.records.Any())
			{
				Console.WriteLine("All records:");
				db.records.ForEach(r => Console.WriteLine(r));
			}
			else
			{
				Console.WriteLine("No records in the database.");
			}
		}
	}
}


public class Ztp01
{
	public static void Main(string[] args)
	{
		//Tworzenie menagera połączeń
		ConnectionManager menager = ConnectionManager.GetInstance();

		//Tworzenie baz danych
		Database database1 = new Database();
		Database database2 = new Database();
		Database database3 = new Database();

		//Tworzenie połączeń z 3 bazami danych
		IDatabaseConnection connection1 = menager.GetConnection(database1);
		IDatabaseConnection connection2 = menager.GetConnection(database2);
		IDatabaseConnection connection3 = menager.GetConnection(database3);

		//Dodawanie rekordów do baz danych
		connection1.AddRecord("Ala", 20);

		connection2.AddRecord("Marek", 21);

		connection3.AddRecord("Kaja", 25);

		connection1.ShowAllRecords();
		connection2.ShowAllRecords();
		connection3.ShowAllRecords();

		Console.WriteLine("\nSprawdzenie działania Object Pool\n");

		//Dodawanie 4 połączeń do jednej bazy
		IDatabaseConnection connection11 = menager.GetConnection(database1);
		IDatabaseConnection connection12 = menager.GetConnection(database1);
		IDatabaseConnection connection13 = menager.GetConnection(database1);
		IDatabaseConnection connection14 = menager.GetConnection(database1);

		connection11.ShowAllRecords();
		connection12.ShowAllRecords();
		connection13.ShowAllRecords();
		connection14.ShowAllRecords();

		Console.WriteLine("\nCzy connection11 jest tym samym obiektem co connection14?");
		Console.WriteLine(Object.ReferenceEquals(connection11, connection14) ? "Tak, connection11 i connection14 to ten sam obiekt." : "Nie, connection11 i connection14 to różne obiekty.");

	}
}