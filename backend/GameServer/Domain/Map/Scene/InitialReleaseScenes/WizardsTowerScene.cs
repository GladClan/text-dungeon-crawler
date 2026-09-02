namespace GameServer.Domain.Map.Scene.InitialReleaseScenes;

/*
public class WizardsTowerSceneState: ISceneState
{
    public bool WizardWarned { get; set; } = false;
    { get; set; } = 
}

public SceneContainer WaterwayScene = new(
    events: [
        { 0, new WizardsTowerEntry() },
    ],
    state: new WizardsTowerSceneState()
);

WizardTowerEntry
    Dialogue
        narrator
            The stone of the hallway becomes more rough. It almost feels like a natural cavern.
            Abruptly, the hallway opens up into a spacious chamber. In the middle of the chamber is a small tower with a torch guttering in front.
            Next to the torch is a staircase spiraling up to the second level of the tower. From there it looks like there is a bridge connecting to the side of the chamber.
    Choices
        Investigate the tower
            // Go to: InvestigateWizardsTower
        Look around the room
            // Go to: WizardsTowerRoom
        Turn back
            // Return to the previous scene

InvestigateWizardsTower
    Dialogue
        narrator
            Patches of grass and bushes grow around the tower, living off the light of the torch outside.
            Approaching the tower, you see a small window in the front, though the interior is obscured by the window’s texture and because it’s dark inside.
            Walking around the tower, it looks like there’s a door on the opposite side, but it won’t open. There is no lock on it.
    Choices
        Continue investigating the tower
            // Go to: ContinueInvestigatingWizardsTower
        Try to knock
            // Go to: KnockOnTheTowerDoor
        Try breaking and entering
            // Go to: BreakAndEnter
        Look around the room

ContinueInvestigatingWizardsTower
    Dialogue
        The tower looks like it has three stories, or three rooms at least.
        Up the tower stairs on the second level, there is some sort of cell. It looks just like a prison cell, there is a barred door.
        The stone is much darker and seems to hold on to shadows.
        You peer into the cell and see something. There’s someone inside.
    Choices
        Talk to the figure
            // Go to: FirstChatWithTel
        Go back to the door
            // Go to: WizardsTowerDoorstep

KnockOnTheTowerDoor


BreakAndEnter


WizardsTowerRoom


FirstChatWithTel


WizardsTowerDoorstep
    Dialogue
        narrator
            The door won't open, but there's no lock on it.
    Choices
        Try to knock
        Try breaking and entering


Continue investigating the tower:
  [Choice to talk to them or move on]
Talk to the figure:
  The figure stirs as you approach. You see that they are dressed in fine clothes and wearing a round, cone-shaped bamboo hat. He is obscured in shadow, so you cannot see his features.
  <Robed figure> Who are you? Who sent you?
  [Ask who they are, what they’re doing in this cell, ask about this dungeon, ask about the tower]
Who they are, what they're doing in this cell:
  The figure looks at you suspiciously for a moment. They seem to be debating internally.
  Finally they speak.
  <Robed figure> You must be travellers. I have seen several groups like you.
  <Tel the wizard> I am Tel. I am a wizard. I have helped many travellers like yourself. But the master of this dungeon discovered my treachery and trapped me in this cell which he created on my tower.
  <Tel the Wizard> You may notice that the stones soak up darkness. They hold me bound, otherwise I would be able to escape with my magic.
  <Tel the Wizard> That evil Turpis must have had help, surely he would not have had the brains to perform this.


Knock:
  There is no response.
  <Mimic speed set to 38>
  [Door surprise]
Breaking and entering:
  You ready yourself.
  The door looks pretty solid. It’s a thick wooden door, but every door can be broken.
  Are you sure about this? You don’t know if someone lives here. And the torch is still burning.
  <Mimic speed set to 18>
Door surprise:
  Something about the door makes you pause.
  There’s something off about it—-it seems to be shifting somehow.
  Suddenly, a gaping hole opens in the middle of the door, like a mouth.
  It is a mouth! The door is a mimic, and as its thick tongue shoots past its razor teeth toward you.
  [《》 Fight: door mimic]
Death of the mimic:
  <receive tower cell key>
  The party is still tense as the mimic’s body lay in what is now obviously an open archway where it had been pretending to be a door.
  [Choice to investigate the room]
Investigate the room:
  Inside is perfectly homely. There is candle guttering in a bracket on the wall, set above a small table laid with a nice, yellow block of cheese and a goblet filled with some sweet-smelling liquid. Some sort of juice, no doubt.
  There doesn’t appear to be anything else of interest in the room.
  [Choice to take the cheese and drink] <obtain cheese wheel and silver goblet>
  [Choice to take the candle from the wall]
Take the candle:
  Really? Take the candle from the wall? Well if you insist, I suppose here you go.
  <obtain candle>
Investigate the rest of the tower:

The wizard is trapped in the tower cell. He congratulates the party on defeating the mimic that was protecting (obstructing?) his home. If the party’s strategy stat is above 50% he sees that they did it from skill rather than luck.
The party can fight the wizard or he can join the party.
The wizard is less likely to join if they looted his home.
The wizard holds the key to the place where the crystal is stored. What does the crystal do? ¯\_(ツ)_/¯


Look around the room:
  text.

*/