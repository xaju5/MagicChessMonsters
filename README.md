# Magic Chess Monsters
 
*Magic Chess Monsters* is a Unity Game Prototype. The idea of the game is to mix together the strategy of a turn-based JRPG like *Pokemon* with *Chess*. 

## Characteristics
- **Turn-based**: Each turn only one character can move or make an action. When they finish, the turn passes to the other player.
- **Chess-based**: The trainer/king is on the battlefield. If they faint or their monsters are defeated, they lose the game.
- **Types and Debility Table**: There are different types of monsters (fire, water, etc.) and each type is effective against and weak to other type.

## Development
This project is currently in a prototyping phase, where many of its mechanics are being defined and tested. 

All the art is a placeholder and it lacks sound effects.

## Gameplay of the First Prototype

In the initial position all the trainers are positioned in the center with their monsters in the sides. It starts the first player.

![Initial position](/mdcontent/StartingPostion.PNG)

After a character is selected, a new UI will appear with :
- The selected Character portrait.
- The detail of the health and Magic points.
- The Attacks available for the character, with relevant information.
- The Tiles where the character can move

![Movement](/mdcontent/MoveArea.PNG)

To make an attack, the player must choose an attack and select an enemy character within range. Then an attack animation will occur and certain amount of damage will be made to the defending character, while consuming Magic points of the attacker. To restore Magic points the character must remain in their position during their turn. Which means using an action with another character.

![Attack](/mdcontent/Attack.gif)

## Credits
The Minions and some attacks placeholders sprites are made by [Pixel Frog](https://pixelfrog-assets.itch.io/tiny-swords) 