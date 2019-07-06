## Important notice

This game is mostly me prototyping around with Unity to make something I can consider a 'game' in the most rudimentary sense.

Do not expect any updates. Do not contribute to this repository. 

If you want to hijack this game, by all means, be my guest and make something nice out of it and let me know what you made. ♥

# Outgrown

A simple 1 screen game about staying alive.

You can create new objects in the folder Resources/Collectibles. Just copy an existing prefab and edit it.

Age refers to an age in your life at which the item is worth the given scores. You should not set this lower than 0 or higher than 100 (since the age checks only include collectibles which are in range (-1, 101) exclusive. An item will only spawn if its minimum is lower than the player's current age, and its maximum is higher than the player's current age. I think they don't need to be sorted since I sort them in-code, but whatever. It's a good convention to sort age scores on age.

The scores themselves should range from -5.0 to 5.0. They are floats. Floats float. Having a more negative score or a more positive score means that it's more likely a score maximally impacts the player's stats.

There was also a small bit of work to include guaranteed checkpoints. These are supposed to run at the moment the player runs this age, instead of randomly appearing.

If all of this is confusing, I apologize, I didn't bother doing any kind of documentation over the game's runtime. You're on your own. It's fairly small, like 600 lines of code total. 
