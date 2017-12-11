using System;
using System.Collections.Generic;
using AMLWorker.Sql;

namespace AMLWorker.A4A
{
    /// <summary>
    /// Top Level App For Answers Query Object
    /// </summary>
    public class A4ARepositoryQuery
    {
        private A4ARepositoryGraphDb db;
        public A4ARepositoryQuery(A4ARepositoryGraphDb db)
        {
            this.db = db;
        }

        /// <summary>
        /// returns a message given an id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public A4AMessage GetMessageById(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search for all messages - optional 'Range' and 'Sort' objects
        /// </summary>
        /// <param name="search"></param>
        /// <param name="range"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public IEnumerable<A4AMessage> SearchMessages(string search, Range range, Sort sort)
        {
            return db.SearchMessages(search, range, sort);
        }


        /// <summary>
        /// Categories represent drill-down lists for experts
        /// </summary>
        public IEnumerable<A4ACategory> Categories { get; set; }


        /// <summary>
        /// Parties are the entities, Experts, Users, Companies
        /// </summary>
        public IEnumerable<A4AParty> Parties { get; set; }
    }
}