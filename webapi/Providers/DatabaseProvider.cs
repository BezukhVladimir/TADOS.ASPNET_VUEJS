namespace Content.Providers
{
    using System.Data.SQLite;

    public static class DatabaseProvider
    {
        public static string ConnectionString { get; private set; }

        public static void Init(string connectionString)
        {
            ConnectionString = connectionString;
            InitSchema();
        }

        private static void InitSchema()
        {
            using SQLiteConnection connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            using SQLiteCommand command = connection.CreateCommand();

            command.CommandText = @"
                PRAGMA foreign_keys = ON;                

                CREATE TABLE IF NOT EXISTS Country (
                    Id   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    Name TEXT    NOT NULL,

                    CONSTRAINT UQ_Country_Name UNIQUE (Name)
                );

                CREATE TABLE IF NOT EXISTS City (
                    Id        INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CountryId INTEGER,
                    Name      TEXT    NOT NULL,

                    CONSTRAINT UQ_City_CountryId_and_Name UNIQUE (CountryId, Name),
                    
                    CONSTRAINT FK_City_Country FOREIGN KEY (CountryId) REFERENCES Country (Id) ON DELETE SET NULL
                );

                CREATE TABLE IF NOT EXISTS User (
                    Id     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CityId INTEGER,
                    Email  TEXT    NOT NULL,
                    
                    CONSTRAINT UQ_User_Email UNIQUE (Email),
                    
                    CONSTRAINT FK_User_City FOREIGN KEY (CityId) REFERENCES City (Id) ON DELETE SET NULL
                );

                CREATE TABLE IF NOT EXISTS Author (
                    UserId INTEGER NOT NULL,
                    
                    CONSTRAINT UQ_Author_UserId UNIQUE (UserId),
                    
                    CONSTRAINT FK_Author_User FOREIGN KEY (UserId) REFERENCES User (Id) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS Rater (
                    UserId INTEGER NOT NULL,
                    
                    CONSTRAINT UQ_Rater_UserId UNIQUE (UserId),
                    
                    CONSTRAINT FK_Rater_User FOREIGN KEY (UserId) REFERENCES User (Id) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS ContentItem (
                    Id       INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    AuthorId INTEGER,
                    Name     TEXT    NOT NULL,
                    Category TEXT    NOT NULL,
                    
                    CONSTRAINT FK_ContentItem_Author FOREIGN KEY (AuthorId) REFERENCES Author (UserId) ON DELETE SET NULL
                );
         
                CREATE TABLE IF NOT EXISTS ArticleItem (
                    ContentItemId INTEGER NOT NULL,
                    Text          TEXT    NOT NULL,
                    
                    CONSTRAINT UQ_ArticleItem_ContentItemId UNIQUE (ContentItemId),
                    
                    CONSTRAINT FK_ArticleItem_ContentItem FOREIGN KEY (ContentItemId) REFERENCES ContentItem (Id) ON DELETE CASCADE
                );
               
                CREATE TABLE IF NOT EXISTS VideoItem (
                    ContentItemId INTEGER NOT NULL,
                    URL           TEXT    NOT NULL,
                    
                    CONSTRAINT UQ_VideoItem_ContentItemId UNIQUE (ContentItemId),
                    
                    CONSTRAINT FK_VideoItem_ContentItem FOREIGN KEY (ContentItemId) REFERENCES ContentItem (Id) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS ImageItem (
                    ContentItemId INTEGER NOT NULL,
                    URL           TEXT    NOT NULL,

                    CONSTRAINT UQ_ImageItem_ContentItemId UNIQUE (ContentItemId),

                    CONSTRAINT FK_ImageItem_ContentItem FOREIGN KEY (ContentItemId) REFERENCES ContentItem (Id) ON DELETE CASCADE
                );

                CREATE TABLE IF NOT EXISTS ImageGalleryItem (
                    ContentItemId INTEGER NOT NULL,
                    CoverId       INTEGER,
                    
                    CONSTRAINT UQ_ImageGalleryItem_ContentItemId UNIQUE (ContentItemId),
                    
                    CONSTRAINT FK_ImageGalleryItem_ContentItem FOREIGN KEY (ContentItemId) REFERENCES ContentItem (Id)          ON DELETE CASCADE,
                    CONSTRAINT FK_ImageGalleryItem_ImageItem   FOREIGN KEY (CoverId)       REFERENCES ImageItem (ContentItemId) ON DELETE SET NULL
                );
                
                CREATE TABLE IF NOT EXISTS GalleryImage (
                    ImageGalleryItemId INTEGER NOT NULL,
                    ImageItemId        INTEGER,
                    
                    CONSTRAINT FK_GalleryImage_ImageGalleryItem FOREIGN KEY (ImageGalleryItemId) REFERENCES ImageGalleryItem (ContentItemId) ON DELETE CASCADE,
                    CONSTRAINT FK_GalleryImage_ImageItem        FOREIGN KEY (ImageItemId)        REFERENCES ImageItem (ContentItemId)        ON DELETE SET NULL
                );
                
                CREATE TABLE IF NOT EXISTS Rating (
                    Id                 INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    RatedContentItemId INTEGER NOT NULL,
                    RaterId            INTEGER,
                    Rate               INTEGER NOT NULL,

                    CONSTRAINT UQ_Rating_RatedContentItemId_and_RaterId UNIQUE (RatedContentItemId, RaterId),
                    
                    CONSTRAINT CK_RateRange CHECK (1 <= Rate AND Rate <= 5),
                    
                    CONSTRAINT FK_Rating_ContentItem FOREIGN KEY (RatedContentItemId) REFERENCES ContentItem (Id) ON DELETE CASCADE,
                    CONSTRAINT FK_Rating_Rater       FOREIGN KEY (RaterId)            REFERENCES Rater (UserId)   ON DELETE SET NULL
                );";

            command.ExecuteNonQuery();
        }
    }
}