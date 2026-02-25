# Where it started

This is a [Next.js](https://nextjs.org) project bootstrapped with [`create-next-app`](https://nextjs.org/docs/app/api-reference/cli/create-next-app).

To run the development server:

```
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

This project uses [`next/font`](https://nextjs.org/docs/app/building-your-application/optimizing/fonts) to automatically optimize and load [Geist](https://vercel.com/font), a new font family for Vercel.


# Upcoming features

- App might be more logical as a React Expo app, using the stack navigation rather than url navigation

## Battle Page

- ❌When going to the battle page, there will be a parameter for whether the party is surrounded (just front/back.. sides would be too complicated)

- Party is displayed on the left of the battle page with their items and abilities which they can use.
    - add a flee button underneath the party, which gives the enemies the chance to attack the party as they flee.
    - Make hidden enemies hidden 🕶️
    - ✅ the items and abilities underneath the party will be their attacks and abilities. will need to pass in the functions through to the buttons from the weapons and such
        - Select a target, then the ability to use.. or vice versa, that might be more intuitive.
        - Considering when characters or the party has the "blind" effect, hides all the enemies so the character can't see who they're targeting. They also have a chance to hit friends 👀
    - ✅ who is active will be emphasized somehow, either size or a scrollbar at the top that rotates the party and enemies according to the initiative list.
        - Make the selected target(s) a different color or glow effect to stand out, and the entity whose turn it is increases in size. Or the other way around? Or different colors.
    - Rather than a single column for the party, there ought to be more than one column for potential guests to the party who come and go as they please and can't be controlled in their actions. 
        - Also pets.
            - *For mobile view, this can switch between the three with a button or switch by the user, and automatically on changing turn.*

- Make an option to bargain during the battle or to use diplomacy or pacify enemies. These can be skills or just options.

## Character

- When creating a character, you will be able to choose the race and class, which come with certain resistances and proficiencies defaultly.
    - Or perhaps through the story you will find people or others and they will join your party. Maybe have an option to bargain during a battle, and that has a chance of the enemy joining the party?? 👀
    - Bargaining (not bargaining, but the other word... negotiation? diplomacy?) for intelligent entities and animal handling for brutish monsters.

- proficiencies are stored in the [Entity Stats](/src/lib/obj/entityParams/entityStats.ts) file.. this might need a cap on it, placed on the levelUp function. Maybe use a logarithmic function?

- On new game, you can choose between several characters, each with different stats and special proficiencies and abilities and inventories, as well as stories.

## Story Page

- The storybox and story page will have an image in the background, setting the scene and the mood and making it look really cool. Also for the battle scene as well.

- Make the story page more of a one-text-at-a-time things, the numerous paragraphs can get overwhelming. Make it like a rpg speech box 🤔 You have an option to see the chat history, but you can also just see the most recent message.

- Make the story page interactable. Through the story, the player can choose options that take them on different paths, like in the beginning, they can turn around and end the game right there, or continue on to find their adventure. Perhaps options come in through a JSON with options and results?

- The main screen will be a map where the rooms appear as you explore. So divs that are hidden until you complete the adjacent div.
    - To make the story more adaptive, rooms can have different states that create different descriptions--'default' state for a room unopened, or 'empty' for a room that has been defeated. Or perhaps a 'recovering' for a room that repeats, like enemies inside regenerating.

## Events
- **<u>Treasure Room event</u>**</br>Gives an awesome weapon and gold to the party, and has an image of a grand chest in the corner of an old, dusty storeroom.