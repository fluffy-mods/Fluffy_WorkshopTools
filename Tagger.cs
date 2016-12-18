using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HugsLib.DetourByAttribute;
using RimWorld;
using Verse;

namespace Fluffy_WorkshopTools.Tagger
{
    public static class Tagger 
    {
        private static Dictionary<ModMetaData, string> _descriptions = new Dictionary<ModMetaData, string>();
        private static Dictionary<ModMetaData, IList<string>> _tags = new Dictionary<ModMetaData,IList<string>>();
        private static Regex tagRegex = new Regex( @"\Atag:" );

        [DetourMethod( typeof( ModMetaData ), "GetWorkshopTags" )]
        public static IList<string> GetWorkshopTags( ModMetaData _this )
        {
            IList<string> tags = new List<string>();

            // try get from cache
            if ( _tags.TryGetValue( _this, out tags ) )
                return tags;
            

            // build new list
            tags = GetTagsFromDescription( _this );
            tags.Add( "Mod" );

            // add to cache
            _tags.Add( _this, tags );
            
#if DEBUG
            Verse.Log.Message( "Fluffy_Workshoptools :: Tags for " + _this.Name + "; " + string.Join( ", ", tags.ToArray() ) );
#endif

            return tags;
        }

        [DetourMethod( typeof(ModMetaData), "GetWorkshopDescription" )]
        public static string GetWorkshopDescription( ModMetaData _this )
        {
            // try get from cache
            string description;
            if ( _descriptions.TryGetValue( _this, out description ) )
                return description;

            // build new list
            description = string.Join( System.Environment.NewLine, GetLines( _this ).Where( l => !IsTagLine( l ) ).ToArray() );
            _descriptions.Add( _this, description );
            
#if DEBUG
            Verse.Log.Message( "Fluffy_Workshoptools :: Description for " + _this.Name + "; " + description );
#endif

            return description;
        }

        public static IList<string> GetTagsFromDescription( ModMetaData mod )
        {
            List<string> tags = new List<string>();
            foreach ( string line in GetLines( mod ) )
                if ( IsTagLine( line ) )
                    tags.Add( GetTagFromLine( line ) );
            
            return tags;
        }

        public static string[] GetLines( ModMetaData mod )
        {
            return mod.Description.Split( new string[] { "\r\n", "\n" }, StringSplitOptions.None );
        }
        public static bool IsTagLine( string line ) { return tagRegex.Match( line ).Success; }
        public static string GetTagFromLine( string line ) { return line.Substring( 4 ); }
    }
}
