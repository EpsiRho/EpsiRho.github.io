---
title: Melon Media Server Devlog 2
description: Melon's Scanner has gotten a big upgrade
slug: mms-dl2
date: 2024-03-08 00:00:00+0000
image: ScannerBanner.png
categories:
    - Blog Post
tags:
    - Windows
    - Linux
    - C-Sharp
    - Devlog
    - Console
---

I have been working really hard on melon the past month, and it's getting close to V1 being feature complete! I've worked on lots of things, but for this post I wanted to talk about the scanner. The last time I talked about Melon the scanner was working but really slow. Especially on my HDD. So I spent awhile looking for optimizations and eventually ended up rewritting the scanner, which I've called Scanner V2.

The old scanner worked like this:
- Load a track off the disk
- Call the database to find out if the track's artist or album exists yet
- If they do, update their info to include the new track, otherwise create new artist / album
- Insert all the new info into the dabase
- Repeat for each track

The new scanner has multiple passes:
- Load tracks
  - Read track info off the disk
  - Create a track object and add into a thread safe List (more on that later)
- Fill out database objects
  - Create album objects based on found tracks
  - Create artist objects based on found tracks
  - Update track info to link to new album and artists objects
- Upload to database
  - Uploads all the in memory info into the database
- Delete 
  - Remove tracks, albums, artists that are missing from the disk

Keeping things in memory sped things up a ton. For the 4607 files on my SSD, it takes about 10-15 seconds. The old scanner took around 3 minutes 30 seconds. 
I was also able to enable loading multiple files at a time with the thread-safe ConcurrentDictionary type. Super useful, but only really works on my SSD. On the HDD it takes longer to load in the file, so the threads get backed up (to a max of 25).

Only a few more things to do for the scanner now. I plan on adding genre merging, because I have a lot of tracks that have genres like Drum and Bass, Drum n Bass, DnB, where they are all the same genre, just written down differently. But they show up as two different genres in the db. So I'd like a way to "Canonicalize" genres into one. 

### Scanner V2 Demo
{{< youtube 4HfW5eiE3tk >}}