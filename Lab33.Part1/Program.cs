using System.Text;
using Lab33.Part1;

// Частина 1. Дослідження механізму серіалізації (варіант 13 — «Товар»).
// Усі 5 завдань виконано в одному проекті.

Console.OutputEncoding = Encoding.UTF8;
DateTime today = DateTime.Today;

// Файли складаємо в окрему папку поруч з exe.
string dir = Path.Combine(AppContext.BaseDirectory, "output");
Directory.CreateDirectory(dir);
string P(string file) => Path.Combine(dir, file);

// ---------------------------------------------------------------
// Завдання 1-2. Клас Product описано (Product.cs) з атрибутом [Serializable].
// Створюємо масив з 3 об'єктів (кількість за варіантом) і серіалізуємо у файл.
// ---------------------------------------------------------------
Console.WriteLine("=== Завдання 2: масив із 3 товарів ===");
Product[] products =
{
    new("Молоко",  today.AddDays(-3),  7),   // придатне
    new("Йогурт",  today.AddDays(-20), 14),  // прострочене
    new("Сир",     today.AddDays(-10), 60),  // придатне
};
Print(products);

Serializers.SaveBinary(P("products.bin"), products);
Console.WriteLine($"Масив серіалізовано у файл {P("products.bin")}\n");

// ---------------------------------------------------------------
// Завдання 3. Новий масив + відновлення в ньому значень із файлу.
// ---------------------------------------------------------------
Console.WriteLine("=== Завдання 3: десеріалізація в НОВИЙ масив ===");
Product[] restored = Serializers.LoadBinary<Product[]>(P("products.bin"));
Print(restored);
Console.WriteLine($"Масиви однакові за вмістом: {products.SequenceEqual(restored)}");
// Важливо: це різні об'єкти в пам'яті (копії), а не ті самі посилання:
Console.WriteLine($"Це той самий об'єкт у пам'яті: {ReferenceEquals(products[0], restored[0])}\n");

// ---------------------------------------------------------------
// Завдання 4. Серіалізація/десеріалізація КОЛЕКЦІЇ (List<Product>)
// та порівняння з масивом.
// ---------------------------------------------------------------
Console.WriteLine("=== Завдання 4: колекція List<Product> ===");
var list = new List<Product>(products);
Serializers.SaveBinary(P("products_list.bin"), list);
List<Product> restoredList = Serializers.LoadBinary<List<Product>>(P("products_list.bin"));
Print(restoredList);

Console.WriteLine($"Тип масиву: {restored.GetType().Name}, довжина: {restored.Length}");
Console.WriteLine($"Тип колекції: {restoredList.GetType().Name}, кількість: {restoredList.Count}");
Console.WriteLine($"Вміст масиву і колекції збігається: {restored.SequenceEqual(restoredList)}");
Console.WriteLine("Висновок: масив і List серіалізуються однаково просто; List гнучкіший " +
                  "(розмір змінюється), масив має фіксований розмір і компактніший у пам'яті.\n");

// ---------------------------------------------------------------
// Завдання 5. Усі види серіалізації: бінарна, XML, JSON, користувацька.
// ---------------------------------------------------------------
Console.WriteLine("=== Завдання 5: порівняння видів серіалізації ===");

// 5.1 Бінарна (вже виконана вище, повторюємо для таблиці результатів)
Report("Бінарна (BinaryFormatter)", P("products.bin"),
       () => Serializers.LoadBinary<Product[]>(P("products.bin")), products);

// 5.2 XML
Serializers.SaveXml(P("products.xml"), products);
Report("XML (XmlSerializer)", P("products.xml"),
       () => Serializers.LoadXml<Product[]>(P("products.xml")), products);

// 5.3 JSON
Serializers.SaveJson(P("products.json"), products);
Report("JSON (System.Text.Json)", P("products.json"),
       () => Serializers.LoadJson<Product[]>(P("products.json")), products);

// 5.4 Користувацька (ISerializable) — використовуємо клас ProductCustom
ProductCustom[] customProducts =
{
    new("Молоко",  today.AddDays(-3),  7),
    new("Йогурт",  today.AddDays(-20), 14),
    new("Сир",     today.AddDays(-10), 60),
};
Serializers.SaveBinary(P("products_custom.bin"), customProducts);
Report("Користувацька (ISerializable)", P("products_custom.bin"),
       () => Serializers.LoadBinary<ProductCustom[]>(P("products_custom.bin")), customProducts);

Console.WriteLine("\nГотово. Файли лежать у папці: " + dir);

// ---------------- допоміжні методи ----------------
void Print(IEnumerable<Product> items)
{
    foreach (var p in items) p.PrintInfo(today);
}

// Десеріалізує файл, виводить вміст, розмір файлу та результат порівняння.
void Report(string title, string path, Func<Product[]> load, Product[] original)
{
    Product[] result = load();
    Console.WriteLine($"\n--- {title} ---");
    Console.WriteLine($"Розмір файлу: {new FileInfo(path).Length} байт");
    Print(result);
    Console.WriteLine($"Збігається з оригіналом: {original.SequenceEqual(result)}");
}
