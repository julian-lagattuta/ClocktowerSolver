using ClocktowerWorldBuilder;
using ClocktowerWorldBuilder.Condition;
using ClocktowerWorldBuilder.Condition.CommonConstraints;
using ClocktowerWorldBuilder.Condition.RoleSpecific.Minion;
using ClocktowerWorldBuilder.Condition.RoleSpecific.Townsfolk;
using ClocktowerWorldBuilder.Constraitns.ElementaryConstraints;
using ClocktowerWorldBuilder.Roles.Minions;
using ClocktowerWorldBuilder.Roles.Outsiders;
using ClocktowerWorldBuilder.Roles.Townsfolk;

Script troubleBrewing = new TroubleBrewing(8); //Start by creating a new TB script with the player count
ClaimWorld claimWorld = new ClaimWorld([ //Containing when everyone died and how
    new Death(new(2, 0), true), //Timestamps consist of a Day (1 indexed) and an Order (0 indexed). The order is important for the order in which nominations occured.
    new Death(new(4,0),true),
    new Alive(),
    new Death(new(3,0),true),
    new Death(new(3,1),false),
    new Alive(),
    new Death(new(1,1),false),
    new Alive(),
], 4, troubleBrewing); // The day parameter represents the current day

// PLAYER IDS ARE ZERO INDEXED BUT ONE INDEXED IN THE OUTPUT

List<ClaimConstraint> claimConstraints = [ //Create a list of every claim
    new ClaimConstraint(0, new(typeof(Washerwoman)), // new RoleTemplate(typeof(Washerwoman)) is how a role is represented.
        new WasherwomanConstraint(1,7, new (typeof(Investigator)))), //The last parameter is the role specific constraint. The parameters are easy to infer
    
    new ClaimConstraint(1, new(typeof(Virgin)),
        new VirginConstraint(3,1,4, true)),
    
    new ClaimConstraint(2, new(typeof(Recluse)),null),
    
    new ClaimConstraint(3, new(typeof(Undertaker)),
        new UndertakerConstraint(2,6, new(typeof(Imp)))),
    
    new ClaimConstraint(4,new(typeof(Chef)),
        new ChefConstraint(1)),
    new ClaimConstraint(5, new(typeof(FortuneTeller)),
        new FortuneTellerConstraint([(1,3,true),(1,5,true),(5,6,true)],claimWorld)),
    
    new ClaimConstraint(6, new(typeof(Slayer)),
        new SlayerConstraint(1,6,5,false,claimWorld)),
    
    new ClaimConstraint(7, new(typeof(Investigator)),
        new InvestigatorConstraint(4,6,new(typeof(Spy)))),
];

Solver gameSolver = new Solver(claimWorld,claimConstraints); //Create a new game solver
var solution = gameSolver.Solve();
solution.PrintAllWorlds();
solution.PrintArrangements();
solution.PrintProbabilities();


//Below are some sample functions you can also use
/*
var filteredSolutions = solution.Filter( //This is a demonstration of the constraint system. Here I am filtering for games where poisoner is in play and player 0 is good
    new AndConstraint([
        new IsAlignment(0,Alignment.Good),
        new RoleNotInPlayConstraint(new(typeof(Poisoner)),  Timestamp.GENESIS) //The second  parameter is the point at which the role died/ceased to exist
    ]));
Console.WriteLine("Here is a summary of just one filtered world:");
var specificWorld = filteredSolutions.Worlds[0];
specificWorld.Summary();
var sortedWorlds = filteredSolutions.SortByArrangements();
*/