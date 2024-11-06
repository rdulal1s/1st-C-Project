using System;

class Character
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int AttackDamage { get; set; }
    public int DefaultHealth { get; set; }
    public int DefaultAttackDamage { get; set; }

    public Character(string name, int health, int attackDamage)
    {
        Name = name;
        Health = health;
        AttackDamage = attackDamage;
        DefaultHealth = health;
        DefaultAttackDamage = attackDamage;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    public virtual void Attack(Character target)
    {
        target.TakeDamage(AttackDamage);
        Console.WriteLine($"{Name} attacks {target.Name} for {AttackDamage} damage!");
    }

    public void ResetStats()
    {
        Health = DefaultHealth;
        AttackDamage = DefaultAttackDamage;
    }
}

class Player : Character
{
    public Player(string name, int health, int attackDamage) : base(name, health, attackDamage) { }

    public void ChooseAction(Character enemy, ref bool changeEnemy)
    {
        Console.WriteLine("Choose an action: 1. Attack  2. Flee  3. Change Enemy (Press 'Q' to quit)");
        string action = Console.ReadLine();

        switch (action)
        {
            case "1":
                Attack(enemy);
                break;
            case "2":
                Console.WriteLine($"{Name} flees the battle!");
                break;
            case "3":
                Console.WriteLine($"{Name} decides to change the enemy!");
                Console.WriteLine("Are you sure you want to change the enemy? (yes/no)");
                string confirmChange = Console.ReadLine().ToLower();
                if (confirmChange == "yes")
                {
                    changeEnemy = true; // Set flag to indicate enemy change
                    Console.WriteLine($"{Name} changes the enemy.");
                    return; // Exit action choice
                }
                break;
            case "Q":
            case "q":
                Console.WriteLine($"{Name} quits the game!");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid action. Try again.");
                ChooseAction(enemy, ref changeEnemy);
                break;
        }
    }

    public void IncreaseStats()
    {
        Health += 50;
        AttackDamage += 10;
        Console.WriteLine($"{Name} has leveled up! Health increased by 50 and Attack Damage increased by 10.");
    }
}

class Enemy : Character
{
    public Enemy(string name, int health, int attackDamage) : base(name, health, attackDamage) { }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Welcome to the Adventure Game!");

        // Ask user for their name
        Console.Write("What is the adventurer's name? ");
        string playerName = Console.ReadLine();

        // Create Player with initial health and attack
        Player player = new Player(playerName, 100, 15);

        // Display initial player stats
        Console.WriteLine($"\nWelcome, {playerName}! Your starting stats are: Health = {player.Health}, Attack Damage = {player.AttackDamage}\n");

        // Create Enemies
        Enemy[] enemies = new Enemy[]
        {
            new Enemy("Orc", 30, 10),
            new Enemy("Pele", 70, 10),
            new Enemy("Saruman", 100, 15),
            new Enemy("Gandalf", 250, 50),
            new Enemy("Robot", 300, 55)
        };

        bool continueGame = true; // Flag to control the game loop

        while (continueGame)
        {
            bool changeEnemy = false; // Flag to indicate if enemy needs to be changed
            Enemy enemy = null;

            // Enemy selection loop
            while (enemy == null)
            {
                Console.WriteLine("Choose an enemy to fight or type 'exit' to quit the game:");
                for (int i = 0; i < enemies.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {enemies[i].Name} (Health: {enemies[i].Health}, Attack: {enemies[i].AttackDamage})");
                }

                string choice = Console.ReadLine();
                if (choice.ToLower() == "exit")
                {
                    Console.WriteLine("Thank you for playing! Goodbye!");
                    return; // Exit the game
                }

                int enemyIndex;
                if (!int.TryParse(choice, out enemyIndex) || enemyIndex < 1 || enemyIndex > enemies.Length)
                {
                    Console.WriteLine("Invalid choice. Try again.");
                    continue;
                }

                enemy = enemies[enemyIndex - 1]; // Select the chosen enemy

                // Warn the player if they choose a powerful enemy
                if (enemy.Health > player.Health || enemy.AttackDamage > player.AttackDamage)
                {
                    Console.WriteLine($"{enemy.Name} is much stronger than you! It is recommended to start with a lower-level enemy.");
                    Console.WriteLine("Do you still want to fight this enemy? (yes/no)");
                    string proceed = Console.ReadLine().ToLower();
                    if (proceed != "yes")
                    {
                        enemy = null; // Reset enemy selection
                        continue;
                    }
                }
            }

            Console.WriteLine($"\nYou chose to fight {enemy.Name}!\n");

            // Combat loop
            bool battleContinue = true;
            while (player.Health > 0 && enemy.Health > 0 && battleContinue)
            {
                player.ChooseAction(enemy, ref changeEnemy); // Get player's action choice

                if (changeEnemy) // If the player chose to change the enemy
                {
                    break; // Exit combat loop to return to enemy selection
                }

                if (player.Health <= 0 || enemy.Health <= 0)
                {
                    break; // Exit combat loop if either player or enemy is defeated
                }

                enemy.Attack(player); // Enemy attacks player
                Console.WriteLine($"{player.Name} takes {enemy.AttackDamage} damage! Remaining Health: {player.Health}");
            }

            // Reset enemy stats after fight if the fight was not interrupted by changing the enemy
            if (enemy.Health <= 0)
            {
                Console.WriteLine($"{enemy.Name} has been defeated!");
                enemy.ResetStats(); // Reset enemy stats
                player.IncreaseStats(); // Level up the player
            }

            // Ask if user wants to continue their journey only after the fight
            if (player.Health > 0)
            {
                Console.WriteLine("Do you want to continue your journey? (yes/no)");
                string continueChoice = Console.ReadLine().ToLower(); // Read player's decision to continue
                if (continueChoice != "yes") // If the player does not want to continue
                {
                    Console.WriteLine("Congratulations on your successful bout!");
                    break; // Exit game loop
                }
            }
            else
            {
                Console.WriteLine("You have been defeated. Game over.");
                break; // End the game if the player is defeated
            }
        }
    }
}
