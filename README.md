# karaboss

Karaboss is a free midi karaoke player and a midi file editor (midi sequencer)
![GitHub Logo](/Gifs/en.player.png)

It is written in C# and run on OS Windows 64b (7, 8/8.1, 10) and mostly based on 2 main code sources :

- The Sanford Multimedia Midi library for the ability to manage midi files
(see http://www.codeproject.com/Articles/6228/C-MIDI-Toolkit and https://github.com/tebjan/Sanford.Multimedia.Midi)


- The Midi Sheet music program developed by Madhav Vaidyanathan for the ability to display music scores.
(see http://midisheetmusic.sourceforge.net/)


Basically you can use this program as a simple Karaoke player.
It displays the lyrics on the screen and you can manage playlists with your favorite songs.
The background of the screen can be modified (solid color, image, diaporama, transparent)
![GitHub Logo](/Gifs/en.karaokewindow.png)

More complex, you can modify the scores, the lyrics or add lyrics to existing songs, because a small MIDI sequencer was added.
![GitHub Logo](/Gifs/editor.png)
Several extensions are added over time: a piano-roll, a virtual piano etc...


The second part, reserved to people knowing music, is the midi sequencer.
You can display and modify the music scores of your files, or create a new one from scratch.
This editor is basic compared to professionnal ones (even the free versions) but enough to have some fun. 
![GitHub Logo](/Gifs/en.explorer.png)

The sources are hard to show because the merger of all these projects into one results in a non-optimized code (to be fair)
Anayway, the program can be freely downloadable at https://karaboss.lacharme.net
