#region Main

var finalScore1 = File.ReadLines("./input.txt")
    .Select(line =>
    {
        var choices = line.Split(" ");

        var opponentChoice = RockPaperScissorsRound.ToOpponentChoice(choices[0]);
        var playerChoice = RockPaperScissorsRound.ToPlayerChoice(choices[1]);

        var round = new RockPaperScissorsRound(playerChoice, opponentChoice);

        return round;
    })
    .Sum(round => round.Score);

Console.WriteLine($"Final score 1: {finalScore1}");


var finalScore2 = File.ReadLines("./input.txt")
    .Select(line =>
    {
        var choiceAndOutcome = line.Split(" ");

        var opponentChoice = RockPaperScissorsRound.ToOpponentChoice(choiceAndOutcome[0]);
        var outcome = RockPaperScissorsRound.ToOutcome(choiceAndOutcome[1]);

        var round = new RockPaperScissorsRound(opponentChoice, outcome);

        return round;
    })
    .Sum(round => round.Score);

Console.WriteLine($"Final score 2: {finalScore2}");

#endregion

public class RockPaperScissorsRound
{
    public Choice PlayerChoice { get; }

    public Choice OpponentChoice { get; }

    public Outcome Outcome { get; }

    public int Score => ChoiceScore + OutcomeScore;

    public int ChoiceScore => PlayerChoice switch
    {
        Choice.Rock => 1,
        Choice.Paper => 2,
        Choice.Scissors => 3,
        _ => throw new ArgumentException("Invalid type of player choice.", nameof(PlayerChoice))
    };

    public int OutcomeScore => Outcome switch
    {
        Outcome.Win => 6,
        Outcome.Draw => 3,
        Outcome.Loss => 0,
        _ => throw new ArgumentException("Invalid type of outcome.", nameof(Outcome))
    };

    public RockPaperScissorsRound(Choice playerChoice, Choice opponentChoice)
    {
        PlayerChoice = playerChoice;
        OpponentChoice = opponentChoice;

        Outcome = this switch
        {
            { PlayerChoice: Choice.Rock, OpponentChoice: Choice.Rock } => Outcome.Draw,
            { PlayerChoice: Choice.Rock, OpponentChoice: Choice.Paper } => Outcome.Loss,
            { PlayerChoice: Choice.Rock, OpponentChoice: Choice.Scissors } => Outcome.Win,

            { PlayerChoice: Choice.Paper, OpponentChoice: Choice.Rock } => Outcome.Win,
            { PlayerChoice: Choice.Paper, OpponentChoice: Choice.Paper } => Outcome.Draw,
            { PlayerChoice: Choice.Paper, OpponentChoice: Choice.Scissors } => Outcome.Loss,

            { PlayerChoice: Choice.Scissors, OpponentChoice: Choice.Rock } => Outcome.Loss,
            { PlayerChoice: Choice.Scissors, OpponentChoice: Choice.Paper } => Outcome.Win,
            { PlayerChoice: Choice.Scissors, OpponentChoice: Choice.Scissors } => Outcome.Draw,

            _ => throw new ArgumentException("Invalid type of choice was provided for either the player or opponent choice.")
        };
    }

    public RockPaperScissorsRound(Choice opponentChoice, Outcome outcome)
    {
        OpponentChoice = opponentChoice;
        Outcome = outcome;

        PlayerChoice = this switch
        {
            { OpponentChoice: Choice.Rock, Outcome: Outcome.Win } => Choice.Paper,
            { OpponentChoice: Choice.Rock, Outcome: Outcome.Draw } => Choice.Rock,
            { OpponentChoice: Choice.Rock, Outcome: Outcome.Loss } => Choice.Scissors,

            { OpponentChoice: Choice.Paper, Outcome: Outcome.Win } => Choice.Scissors,
            { OpponentChoice: Choice.Paper, Outcome: Outcome.Draw } => Choice.Paper,
            { OpponentChoice: Choice.Paper, Outcome: Outcome.Loss } => Choice.Rock,

            { OpponentChoice: Choice.Scissors, Outcome: Outcome.Win } => Choice.Rock,
            { OpponentChoice: Choice.Scissors, Outcome: Outcome.Draw } => Choice.Scissors,
            { OpponentChoice: Choice.Scissors, Outcome: Outcome.Loss } => Choice.Paper,

            _ => throw new ArgumentException("Invalid type of opponent choice or outcome was provided.")
        };
    }

    public static Choice ToPlayerChoice(string choice)
    {
        return choice switch
        {
            "X" => Choice.Rock,
            "Y" => Choice.Paper,
            "Z" => Choice.Scissors,
            _ => throw new ArgumentException("Invalid type of player choice.", nameof(choice))
        };
    }

    public static Choice ToOpponentChoice(string choice)
    {
        return choice switch
        {
            "A" => Choice.Rock,
            "B" => Choice.Paper,
            "C" => Choice.Scissors,
            _ => throw new ArgumentException("Invalid type of opponent choice.", nameof(choice))
        };
    }

    public static Outcome ToOutcome(string outcome)
    {
        return outcome switch
        {
            "X" => Outcome.Loss,
            "Y" => Outcome.Draw,
            "Z" => Outcome.Win,
            _ => throw new ArgumentException("Invalid type of outcome.", nameof(outcome))
        };
    }
}

public enum Choice
{
    Rock,
    Paper,
    Scissors
}

public enum Outcome
{
    Win,
    Loss,
    Draw
}