using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace NMC.DataAccess
{
    public static class DbInitializer
    {
        public static void InitializeDatabase(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var script = @"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
            BEGIN
                CREATE TABLE Users (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Username NVARCHAR(100) NOT NULL UNIQUE,
                    PasswordHash NVARCHAR(255) NOT NULL
                );

                CREATE TABLE Tracks (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Title NVARCHAR(200) NOT NULL,
                    Artist NVARCHAR(200) NOT NULL
                );

                CREATE TABLE UserTracks (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    TrackId INT NOT NULL,
                    UserId INT NOT NULL,
                    FilePath NVARCHAR(500) NOT NULL,
                    CoverPath NVARCHAR(500) NULL,
                    CONSTRAINT FK_UserTracks_Tracks FOREIGN KEY (TrackId) REFERENCES Tracks(Id) ON DELETE CASCADE,
                    CONSTRAINT FK_UserTracks_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );

                CREATE TABLE Genres (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL UNIQUE
                );

                CREATE TABLE GenreScores (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    TrackId INT NOT NULL,
                    GenreId INT NOT NULL,
                    Score FLOAT NOT NULL,
                    CONSTRAINT FK_GenreScores_Tracks FOREIGN KEY (TrackId) REFERENCES Tracks(Id) ON DELETE CASCADE,
                    CONSTRAINT FK_GenreScores_Genres FOREIGN KEY (GenreId) REFERENCES Genres(Id) ON DELETE CASCADE
                );

                CREATE TABLE AcousticAttributes (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL UNIQUE
                );

                CREATE TABLE AcousticScores (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    TrackId INT NOT NULL,
                    AttributeId INT NOT NULL,
                    Score FLOAT NOT NULL,
                    CONSTRAINT FK_AcousticScores_Tracks FOREIGN KEY (TrackId) REFERENCES Tracks(Id) ON DELETE CASCADE,
                    CONSTRAINT FK_AcousticScores_AcousticAttributes FOREIGN KEY (AttributeId) REFERENCES AcousticAttributes(Id) ON DELETE CASCADE
                );

                CREATE TABLE Playlists (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    UserId INT NOT NULL,
                    PlaylistName NVARCHAR(200) NOT NULL,
                    CoverPath NVARCHAR(500) NULL,
                    CONSTRAINT FK_Playlists_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );

                CREATE TABLE PlaylistTracks (
                    UserTrackId INT NOT NULL,
                    PlaylistId INT NOT NULL,
                    PRIMARY KEY (UserTrackId, PlaylistId),
                    CONSTRAINT FK_PlaylistTracks_UserTracks FOREIGN KEY (UserTrackId) REFERENCES UserTracks(Id),
                    CONSTRAINT FK_PlaylistTracks_Playlists FOREIGN KEY (PlaylistId) REFERENCES Playlists(Id) ON DELETE CASCADE
                );
            END;
            ";

            using var command = new SqlCommand(script, connection);
            command.ExecuteNonQuery();
        }
    }
}
