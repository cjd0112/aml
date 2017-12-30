using System;
using System.Collections.Generic;
using As.GraphDB.Sql;
using As.Shared;

namespace As.A4ACore
{
    public interface IA4ARepository
    {
        GraphResponse RunQuery(GraphQuery query);

        T AddObject<T>(T party);
        IEnumerable<T> QueryObjects<T>(string query="",Range r=null,Sort s=null);
        T GetObjectByPrimaryKey<T>(string id);
        T SaveObject<T>(T party);
        void DeleteObject<T>(string id);
        int Count<T>();

        IEnumerable<(ForeignKey foreignKey, IEnumerable<string> values)> GetPossibleForeignKeys<T>();


        (A4AUser user,IEnumerable<A4AExpert> experts) GetUserAndExpertsForMessage(A4AMessage msg);

        A4AEmailRecord UpdateEmailRecordStatus(string externalMessageId, string status);

        (A4AUser user, A4AExpert expert) GetUserAndExpertForReply(string fromEmail, string toEmail);


        Mailbox GetMailbox(MailboxRequest request);

    }
}