using System;

internal static class Program {
    private static void Main() {
        SimpleBuilder.Create().Id(1).Name("Riku").Age(17).Build().Show();
        SimpleBuilder.Create().Id(2).Build().Show(); // (no name, no id)
        SimpleBuilder.Create().Id(3).Name("Kimi").Name("Maria").Build().Show(); // (no id, two names)

        Builder.Create().Id(4).Name("Aki").Build().Show();
        Builder.Create().Name("Anna").Id(5).Build().Show();
        Builder.Create().Name("Mika").Age(24).Id(6).Build().Show();
//      Builder.Create().Id(7).Name("Masa").Name("Jarkko").Build(); // type error. (two names)
//      Builder.Create().Id(8).Build(); // type error. (no name)
    }
}

internal class Person {
    private readonly int    _id;
    private readonly string _name;
    private readonly int?   _age;

    public Person(int id, string name, int? age) => (_id, _name, _age) = (id, name, age);

    public void Show() => Console.WriteLine(
        _age == null
            ? $"id = {_id}, name = {_name}"
            : $"id = {_id}, name = {_name}, age = {_age}");
}

internal class SimpleBuilder {
    private readonly int    _id;
    private readonly string _name;
    private readonly int?   _age;

    private SimpleBuilder(int id, string name, int? age) => (_id, _name, _age) = (id, name, age);

    public static SimpleBuilder Create() => new SimpleBuilder(default(int), default(string), default(int?));

    public SimpleBuilder Id(int      id)   => new SimpleBuilder(id,  _name, _age);
    public SimpleBuilder Name(string name) => new SimpleBuilder(_id, name,  _age);
    public SimpleBuilder Age(int     age)  => new SimpleBuilder(_id, _name, age);

    public Person Build() => new Person(_id, _name, _age);
}

internal static class Builder {
    public static Builder<None, None, None> Create() =>
        new Builder<None, None, None>(default(int), default(string), default(int?));

    public static Builder<Some, TName, TAge> Id<TName, TAge>(this Builder<None, TName, TAge> builder, int id)
        where TName : IOpt where TAge : IOpt =>
        new Builder<Some, TName, TAge>(id, builder.Name, builder.Age);

    public static Builder<TId, Some, TAge> Name<TId, TAge>(this Builder<TId, None, TAge> builder, string name)
        where TId : IOpt where TAge : IOpt =>
        new Builder<TId, Some, TAge>(builder.Id, name, builder.Age);

    public static Builder<TId, TName, Some> Age<TId, TName>(this Builder<TId, TName, None> builder, int age)
        where TId : IOpt where TName : IOpt =>
        new Builder<TId, TName, Some>(builder.Id, builder.Name, age);

    public static Person Build<TAge>(this Builder<Some, Some, TAge> builder) where TAge : IOpt =>
        new Person(builder.Id, builder.Name, builder.Age);
}

internal class Builder<TId, TName, TAge> where TId : IOpt where TName : IOpt where TAge : IOpt {
    internal readonly int    Id;
    internal readonly string Name;
    internal readonly int?   Age;

    internal Builder(int id, string name, int? age) => (Id, Name, Age) = (id, name, age);
}

internal interface IOpt {}

internal abstract class None : IOpt {}

internal abstract class Some : IOpt {}
