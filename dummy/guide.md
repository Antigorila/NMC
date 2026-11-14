# Demo data

## Edge cases in the tags

| File | Tag state | What it tests |
|---|---|---|
| So Alone.mp3 | no title, no artist, no cover | empty Add track form, required field validation on the API |
| Specter.mp3, Doomed.mp3, And I Return to Nothingness.mp3 | no artist | missing artist, the user has to type it |
| Currents - Remember Me.mp3 | no title | missing title |
| Sleep Token - Damocles.mp3 | title is `<unknown>` | placeholder title is not in the API's blocked list, so it is accepted |
| Sleep Token - Emergence.mp3 | artist is `Unknown` | the API rejects generic names |
| BAD OMENS - The Grey.mp3 | artist has leading and trailing spaces | trimming on the API |
| BAD OMENS - THE DEATH OF PEACE OF MIND.mp3 | artist `bad omens`, title in capitals | case-insensitive artist reuse |
| ATLAS, LORNA SHORE, BAD OMENS - ARTIFICIAL SUICIDE | artist in capitals | same as above |
| Motionless in White / Motionless In White | two spellings across files | artist name reuse |
| Motionless in White - R.I.P. (feat. Skylar Grey).mp3 | two artists | only the first artist is read |
| Playing God (ft. Corey Taylor), R.I.P. (feat. ...) | featuring in the title | duplicate check by exact title |
| Dying To Love, Count Back From Zero, Playing God, R.I.P. | no embedded cover | placeholder cover |
| Spiritbox - Sun Killer.mp3 | 1.5 MB PNG cover | large embedded image |
| covers/*.png | most files are JPEG data with a .png name | image loading by content, not extension |

## Demo users

| Username | Password | Tracks | Playlists |
|---|---|---|---|
| anna | anna123 | 8 (Sleep Token, Spiritbox, Atlas) | Even In Arcadia (3), Spiritbox favourites (2), Late night (3) |
| bence | bence123 | 9 (Bad Omens, Bring Me The Horizon) | Bad Omens essentials (4), Gym (4) |
| csilla | csilla123 | 9 (Motionless In White, plus Like A Villain and Gods) | Motionless (5) |
| david | david123 | 7 (Lorna Shore, Currents, Atlas, MIW, Spiritbox) | Deathcore (2), Mixed (4) |
| eszter | eszter123 | 7 (mixed) | Favourites (4), Chill (2, no cover), Empty playlist (0) |

Shared tracks (more users have them): 
    Bittersleep
    Like A Villain
    The Death of Peace of Mind
    Throne, Remember Me
    Disguise
    Playing God
    Voices
    Even In Arcadia
    Gods
    Take Me Back To Eden
    No Loss No Love
    Sun Killer