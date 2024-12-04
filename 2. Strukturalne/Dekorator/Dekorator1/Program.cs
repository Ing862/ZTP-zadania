using System;
using System.Collections.Generic;
using System.ComponentModel;

public interface IMessage
{
	int Id { get; set; }
	string Title { get; set; }
	string Content { get; set; }
}



public class Message
{
	public int Id { get; set; }
	public string Title { get; set; }
	public string Content { get; set; }

	public Message(string title, string content)
	{
		Title = title;
		Content = content;
	}
}

public interface IMessageBox
{
	void AddMessage(Message message);
	Message GetMessageById(int id);
	void DisplayAllMessageTitles();

}


public class MessageBox : IMessageBox
{
	private List<Message> messages = new List<Message>();
	private int nextId = 1;

	public void AddMessage(Message message)
	{
		message.Id = nextId++;
		messages.Add(message);
	}

	public Message GetMessageById(int id)
	{
		return messages.Find(m => m.Id == id);
	}

	public void DisplayAllMessageTitles()
	{
		Console.WriteLine("\nLista wiadomości:");
		if (messages.Count == 0)
		{
			Console.WriteLine("Brak wiadomości w skrzynce.");
		}
		else
		{
			foreach (var message in messages)
			{
				Console.WriteLine($"ID: {message.Id} - {message.Title}");
			}
		}
	}
}

public class MessageBoxDecorator :IMessageBox
{
	protected IMessageBox _messageBox;
	public MessageBoxDecorator(IMessageBox messageBox)
	{
		this._messageBox = messageBox;
	}
	public virtual void AddMessage(Message message)
	{
		_messageBox.AddMessage(message);
	}
	public virtual Message GetMessageById(int id)
	{
		_messageBox.GetMessageById(id);
	}
	public virtual void DisplayAllMessageTitles()
	{
		_messageBox.DisplayAllMessageTitles();
	}

} 

class Program
{
	static void Main(string[] args)
	{
		MessageBox messageBox = new MessageBox();

		// Dodanie przykładowych wiadomości
		messageBox.AddMessage(new Message("Powiadomienie o spotkaniu", "Spotkanie zespołu odbędzie się w piątek o godzinie 10:00."));
		messageBox.AddMessage(new Message("Nowe zasady pracy zdalnej", "Od przyszłego miesiąca obowiązują nowe zasady pracy zdalnej."));
		messageBox.AddMessage(new Message("Wyniki kwartalne", "Wyniki finansowe za ostatni kwartał pokazują wzrost o 15%."));

		bool running = true;
		while (running)
		{
			// Wyświetlanie wszystkich tematów wiadomości
			messageBox.DisplayAllMessageTitles();

			Console.WriteLine("\nWybierz ID wiadomości do wyświetlenia (lub 0, aby zakończyć): ");
			if (int.TryParse(Console.ReadLine(), out int id))
			{
				if (id == 0)
				{
					running = false;
					Console.WriteLine("Koniec programu.");
				}
				else
				{
					var message = messageBox.GetMessageById(id);
					if (message != null)
					{
						Console.WriteLine($"\nTytuł: {message.Title}");
						Console.WriteLine($"Treść: {message.Content}");
					}
					else
					{
						Console.WriteLine("Nie znaleziono wiadomości o podanym ID.");
					}
				}
			}
			else
			{
				Console.WriteLine("Nieprawidłowy wybór.");
			}
		}
	}
}
