namespace GameServer.Domain.Map.Scene.InitialReleaseScenes;

/*
public class WaterwaySceneState: ISceneState
{
    public bool FiremaneDefeated { get; set; } = false;
    public bool WaterwayFlooded { get; set; } = false;
    public int CrystalsPressed { get; set; } = 0;
    public bool ExitGateOpen { get; set; } = false;
    public bool WaterwayGrateOpen { get; set; } = false;
    public int AttemptsToOpenWaterwayGrate { get; set; } = 0;
    public bool PartyHasRested { get; set; } = false;
}

public SceneContainer WaterwayScene = new(
    events: [
        { 0, new WaterwayEntry() },
        { 1, new WaterwayRoomDescription() },
        { 2, new WaterwayExitGate() },
        { 3, new InvestigateWaterwayGrates() },
        { 4, new InvestigateGratesWaterwayFlooded() },
        { 5, new WaterwayGrateOpeningAttempt() },
        { 6, new FailToPickLockOfWaterwayGrate() },
        { 7, new OpenWaterwayGrate() },
        { 8, new WaterwayControlPedastal() },
        { 9, new PressWaterwayPedestalCrystal() },
        { 10, new FirstWaterwayPedestalCrystalPressed() },
        { 11, new PressSecondWaterwayPedestalCrystal() },
        { 12, new DefeatOfTheFiremane() },
        { 13, new AllPedestalCrystalsPressed() },
        { 14, new PedestalReset() },
        { 15, new Restful() },
        { 16, new RestfulAfterBattle() },
    ],
    state: new WaterwaySceneState()
);

WaterwayEntry
    Dialogue
        narrator
            As the party continues along the stone hallway, they hear a sound like flowing water.
            The party approaches an ornate metal gate with silvery-blue light glowing from the other side.
            There is a distant roaring of water flowing, along with a nearer sound of gentle lapping, like a small river.
    Choices
        turn around
            // Return to previous scene
        open the gate and enter the room
            // Go to event: WaterwayRoomDescription

WaterwayRoomDescription
    Dialogue
        narrator
            The room is large and square. The party stands on a small balcony on the other side of the gate with a bridge leading to a large circular platform in the center of the room.
                The platform is big enough for the whole party to spread out.
            On the opposite side of the room there is another balcony also connecting to the center platform.
            Water is flowing around the platform about eight feet below. The stream runs from a grate on the left side of the room to a similar grate on the right side.
            On the left side of the center platform is a short pedestal made of a gilded bronze material. Near the pedestal is a ladder descending the short distance to the flowing water.
            This seems like a good place to take a rest. The water looks clear and smells fresh.
    Choices
        Rest
            // CONDITION: PartyHasRested == false
            // EFFECT: Heal party and recover mana
            // Go to: Restful
        Investigate the grate
            // Go to event: InvestigateWaterwayGrates
        Investigate the pedestal
            // Go to event: WaterwayControlPedastal
        Try the other gate
            // Go to event: WaterwayExitGate
        // Return to the previous location

WaterwayExitGate
    Dialogue
        narrator
            The gate on the opposite side of the room is locked.
            The lock has no keyhole, but it looks mechanical and has a crystal set into the top of it.
            You notice that the pedestal in the middle of the room has matching crystals.
    Choices
        Rest
        Investigate the grate
        Investigate the pedestal
        // Return to the previous location

InvestigateWaterwayGrates
    Condition
        WaterwayFlooded = false
    IfNotMetRequirements
        // Go to: InvestigateGratesWaterwayFlooded
    Dialogue
        narrator
            The water at the bottom of the ladder isn’t that deep. It hardly comes up to your ankles.
            The grates are a solid gray metal with vertical bars. The grate on the left side has a small hinge on the top.
    Choices
        Try to open it
            // Go to WaterwayGrateOpeningAttempt
        Rest
        Investigate the pedestal
        Try the other gate
        // Return to the previous location

InvestigateGratesWaterwayFlooded
    Condition
        WaterwayFlooded = true
    IfNotMetRequirements
        // Go to: InvestigateWaterwayGrates
    Dialogue
        narrator
            The water is flowing as high as halfway up the ladder, it isn't safe to climb down to investigate the grates at this point.
            Perhaps investigating the pedestal further would reveal something of how it works...
    Choices
        Rest
        Investigate the pedestal
        Try the other gate
        // Return to the previous location

WaterwayGrateOpeningAttempt
    Dialogue
        narrator
            It has a heavy lock on it.
            You see that the silvery light is coming from deeper in the grate.
            From what you can see, it looks like this is an entrance into the waterways.
    Choices
        Open the lock
            // CONDITION: HasItem("waterway-key)
            // Go to: OpenWaterwayGrate
        Try to pick the lock
            // if (Skill check: stealth[difficulty = {hard + AttemptsToOpenWaterwayGrate}] == success)
            // EFFECT: WaterwayGrateOpen = true
            // Go to: OpenWaterwayGrate
            // else
            // EFFECT: AttemptsToOpenWaterwayGrate++
            // Go to: FailToPickLockOfWaterwayGrate
        Rest
        Try the other gate
        Investigate the pedestal

FailToPickLockOfWaterwayGrate
    Dialogue
        narrator
            You can tell the water running past has rusted and corroded the lock. It's not going to come open easily.
    Choices
        Rest
        Try the other gate
        Investigate the pedestal

OpenWaterwayGrate
    Dialogue
        narrator
            You can tell the water running past has rusted and corroded the lock.
            It doesn't come open easily. But you manage to twist it until it unlatches.
            Looking in, the waterways are dimly lit by some sort of silvery crystal.
    Choices
        Enter the waterways
            // Go to: new scene in the Waterways
        Rest
        Try the other gate
        Investigate the pedestal
        // Return to the previous location

WaterwayControlPedastal
    Condition
        CrystalsPressed == 0 && FiremaneDefeated == false
    IfNotMetRequirements
        Go to: FirstWaterwayPedestalCrystalPressed
    Dialogue
        narrator
            There are three crystals set into the pedestal like buttons.
            One of them is inset deeper than the others and glows with the silvery moonlight.
    Choices
        Press a crystal
            // EFFECT: CrystalsPressed = 1, WaterwayFlooded = true
            // Go to: PressedWaterwayPedestalCrystal
        Rest
        Try the other gate
        Investigate the grate
        // Return to the previous location

PressWaterwayPedestalCrystal
    Dialogue
        narrator
            At your press, the crystal sinks easily into the pedestal.
            The water flowing around the platform increases in speed and the water rises halfway to the pedestal.
            The water is high enough and it's flowing fast enough that it doesn't seem wise to climb down anymore
            Thank goodness for those bridges, you can still go back or forward to either gate.
    Choices
        Press the other crystal
            // EFFECT: CrystalsPressed = 2
            // Go to: PressSecondWaterwayPedestalCrystal
        Rest
        Try the other gate
        // Return to the previous location

FirstWaterwayPedestalCrystalPressed
    Condition
        CrystalsPressed == 1 && FiremaneDefeated == false
    IfNotMetRequirements
        Go to: AllPedestalCrystalsPressed
    Dialogue
        narrator
            There are three crystals set into the pedestal like buttons.
            Two of them are inset deeper than the others and glows with the silvery moonlight. One you pressed and one that was alread pressed.
    Choices
        Press the other crystal
        Rest
        Try the other gate
        // Return to the previous location

PressSecondWaterwayPedestalCrystal
    Dialogue
        narrator
            The water level rises once more, trickling just over the edge of the platform.
            You hear a mechanical click on the other side of the room as a matching crystal set into the left grate begins to glow.
            The color of the glowing crystals change, becoming more ruddy and yellow.
            The room begins to grow warm as the ornate gate you entered from swings shut on its own. You see a lock click into place on the top, sealing you in.
            Steam begins to rise from the water.
            As the steam begins to make the room hazy, you see a sphere of orange light glowing in the center of the room.
            The sphere grows brighter and larger, becoming more distinct. It begins to take the shape of a horse with fiery tendrils streaming from its back.
            The entity rears back, swinging its head to look at your party. It is obviously enraged, but it's unclear why. It is clear you are going to have to fight the Firemane.
    Choices
        Prepare for battle!
            // Fight the firemane
            // [OnBattleEnd]
                // PartyHasRested = false
                // FiremaneDefeated = true
                // Go to: DefeatOfTheFiremane

DefeatOfTheFiremane
    Dialogue
        narrator
            As the party relaxes after the danger of the Firemane is past, you hear a tinkling crack.
            The crystal above the locked gate shatters and its tiny shards litter the ground.
            The gate starts to swing loosely on its hinges. It appears the way forward is now open.
    Choices
        Collect the crystal shards
            // EFFECT: GiveItem(waterway-crystal-shards)
        Investigate the pedesatal again
            // Go to: AllPedestalCrystalsPressed
        Rest
        Continue through the other gate
            // CONDITION: ExitGateOpen == true
            // Go to: Next scene
        // Return to the previous location

AllPedestalCrystalsPressed
    Condition
        CrystalsPressed == 2 && FiremaneDefeated == true
    IfNotRequirementsMet
        // Go to: PedestalReset
    Dialogue
        narrator
        All of the crystals are depressed into the face of the pedestal.
    Choices
        Use the crystal shards
            // CONDITION: HasItem("waterway-crystal-shards")
            // EFFECT: CrystalsPressed = 0, WaterwayFlooded = false
            // Go to: PedestalReset
        Rest
        Continue through the other gate
        // Return to the previous location

PedestalReset
    Condition
        FiremaneDefeated == true
    IfNotRequirementsMet
        // I don't know, man... How did you get here?
    Dialogue
        narrator
            The two crystals that you pressed earlier rise back up as they come in proximity to the shards of the crystal locking the gate.
            The waters flowing past slow and recede down to their previous height.
    Choices
        Investigate the grate
        Rest
        Continue through the other gate
        // Return to the previous location


Restful
    Condition
        FiremaneDefeated = false
    IfRequirementsNotMet
        // Go to: RestfulAfterBattle
    Dialogue
        narrator
            The party lays out some sleeping equipment on the ground.
            The sound of flowing water make this a nice area to rest for a moment and recover from all you've encountered in this place.
            You feel refreshed, your aches and pains lessening slightly and your reserves of magic rejuvenating.
    Choices
        Investigate the grate
        Investigate the pedestal
        Try the other gate
        // Return to the previous location

RestfulAfterBattle
    Dialogue
        narrator
            The party lays out some sleeping equipment on the ground.
            After that battle, the sound of flowing water feels more ominous...
            Nevertheless, you manage to take a moment to rest and recover from your encounters.
            You feel refreshed, your aches and pains lessening slightly and your reserves of magic rejuvenating.
    Choices
        Investigate the grate
        Investigate the pedestal
        Try the other gate
        // Return to the previous location
*/