using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex10_MovieFinder2024.Models
{
    public class MovieItem
    {
        public bool Adult { get; set; }

        public int Id { get; set; }
        public string Original_Language { get; set; }
        public string Original_Title { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string Poster_Path { get; set; }
        public string Release_Date { get; set; }
        public string Title { get; set; }
        public double Vote_Average { get; set; }
        public int Vote_Count { get; set; }

        public DateTime Reg_Date { get; set; }  // 최초에는 없기때문에 Nullable 지정

        // 쿼리파트
        public static readonly string SELECT_QUERY = @"[Id] int NOT NULL PRIMARY KEY,
                                                       ,[Title] nvarchar(300) NOT NULL,
                                                       ,[Original_Title] nvarchar(300) NOT NULL,
                                                       ,[Release_Date] char(10) NOT NULL,
                                                       ,[Original_Language] varchar(10) NOT NULL,
                                                       ,[Adult] bit NULL,
                                                       ,[Popularity] float NOT NULL,
                                                       ,[Vote_Average] float NOT NULL,
                                                       ,[Vote_Count] int NOT NULL,
                                                       ,[Poster_Path] varchar(300) NULL,
                                                       ,[Overview] ntext NULL,
                                                       ,[Reg_Date] datetime NOT NULL";
        public static readonly string INSERT_QUERY = @"INSERT INTO[dbo].[MovieItem]
                                                                ([Id]
                                                               , [Title]
                                                                , [Original_Title]
                                                                , [Release_Date]
                                                                , [Original_Language]
                                                                , [Adult]
                                                                , [Popularity]
                                                                , [Vote_Average]
                                                                , [Vote_Count]
                                                                , [Poster_Path]
                                                                , [Overview]
                                                                , [Reg_Date])
                                                              VALUES
                                                                (@Id
                                                                , @Title
                                                                , @Original_Title
                                                                , @Release_Date
                                                                , @Original_Language
                                                                , @Adult
                                                                , @Popularity
                                                                , @Vote_Average
                                                                , @Vote_Count
                                                                , @Poster_Path
                                                                , @Overview
                                                                , GETDATE())";
        //public static readonly string UPDATE_QUERY = @"";
        public static readonly string DELETE_QUERY = @"DELETE FROM [dbo].[MovieItem].[MovieItem] WHERE Id = @Id";
    }
}
