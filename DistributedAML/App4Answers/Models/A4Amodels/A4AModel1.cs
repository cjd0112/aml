using System;
using System.Collections.Generic;
using System.Linq;
using App4Answers.Models.A4Amodels.Administration;
using App4Answers.Models.A4Amodels.Base;
using App4Answers.Models.A4Amodels.EmailManager;
using App4Answers.Models.A4Amodels.Login;
using As.A4ACore;
using As.Email;
using As.GraphDB.Sql;
using As.Logger;
using As.Shared;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{
    public class A4AModel1
    {
        public A4ARepository Repository { get; private set; }

        public ModelNames.ObjectTypes ObjectTypes { get; set; }
        public ModelNames.Verb Verb { get; set; }
        private HttpContextAccessor accessor;
        private IEmailSender sender;
        public A4AModel1(A4ARepository repository,HttpContextAccessor accessor,IEmailSender sender )
        {
            this.Repository = repository;
            this.accessor = accessor;
            this.sender = sender;

        }

        public A4AEmailService GetEmailDefinition()
        {
            var emailService = Repository.GetObjectByPrimaryKey<A4AEmailService>("mailgun");
            if (emailService == null)
                throw new Exception(
                    "Could not load email service settings from database - looking for 'mailgun' entry in 'A4AEmailService' table");

            return emailService;
        }

        public void SaveEmailDefinition(A4AEmailService service)
        {
            Repository.SaveObject(service);
        }

        private String GetUserEmail()
        {
            return accessor.HttpContext.Session.GetString(ModelNames.SessionStrings.UserEmail.ToString());
        }

        private A4AUserType GetUserType()
        {
            return (A4AUserType) Enum.Parse(typeof(A4AUserType),accessor.HttpContext.Session.GetString(ModelNames.SessionStrings.UserType.ToString()));

        }

        private String GetUserName()
        {
            return Repository.GetObjectByPrimaryKey<A4AAuthenticationAccount>(GetUserEmail()).Name;
        }

        public A4ALoginViewModel Login(A4ALoginViewModel vm)
        {
            var aa = Repository.GetObjectByPrimaryKey<A4AAuthenticationAccount>(vm.Email);
            if (aa == null)
            {
                // account does not exist
                vm.Authenticated = A4ALoginViewModel.AuthenticationResult.UserNotFound;
            }
            else
            {
                vm.AuthenticationAccount = aa;

                if (vm.Code1 == aa.Code1 && vm.Code2 == aa.Code2 && vm.Code3 == aa.Code3 && vm.Code4 == aa.Code4)
                {
                    vm.Authenticated = A4ALoginViewModel.AuthenticationResult.Authenticated;
                }
                else
                {
                    vm.Authenticated = A4ALoginViewModel.AuthenticationResult.PasswordInvalid;
                }
            }
            return vm;
        }

        #region COMPANY


        public A4ACompanyDetailViewModel NewCompany()
        {
            return new A4ACompanyDetailViewModel(new A4ACompany())
                .AddForeignKeys<A4ACompanyDetailViewModel>(Repository.GetPossibleForeignKeys<A4ACompany>());
        }

        public ViewModelListBase ListCompany()
        {
            return new ViewModelListBase(typeof(A4ACompanySummaryViewModel), Repository
                .QueryObjects<A4ACompany>()
                .Select(x => new A4ACompanySummaryViewModel(x)));
        }

        public A4ACompanyDetailViewModel EditCompany(string id)
        {
            return new A4ACompanyDetailViewModel(Repository.GetObjectByPrimaryKey<A4ACompany>(id))
                .AddForeignKeys<A4ACompanyDetailViewModel>(Repository.GetPossibleForeignKeys<A4ACompany>());
        }

        public ViewModelListBase SaveCompany(IFormCollection form)
        {
            var party = Repository.SaveObject(new A4ACompanyDetailViewModel(form).ModelClassFromViewModel());
            return ListCompany();
        }

        public ViewModelListBase DeleteCompany(String id)
        {
            Repository.DeleteObject<A4ACompany>(id);
            return ListCompany();
        }
        #endregion

        #region EXPERT
        public ViewModelListBase ListExpert()
        {
            return new ViewModelListBase(typeof(A4AExpertSummaryViewModel), Repository
                    .QueryObjects<A4AExpert>()
                    .Select(x => new A4AExpertSummaryViewModel(x)));
        }

        public A4AExpertDetailViewModel NewExpert()
        {
            return new A4AExpertDetailViewModel(new A4AExpert(), ModelNames.Verb.New).AddForeignKeys<A4AExpertDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4AExpert>());
        }

        public A4AExpertDetailViewModel EditExpert(string id)
        {
            return new A4AExpertDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4AExpert>(id),
                    ModelNames.Verb.Edit)
                .AddForeignKeys<A4AExpertDetailViewModel>(Repository.GetPossibleForeignKeys<A4AExpert>());
        }

        public ViewModelListBase SaveExpert(IFormCollection form)
        {
            var expert = Repository.SaveObject(new A4AExpertDetailViewModel(form).ModelClassFromViewModel());
            return ListExpert();

        }

        public ViewModelListBase DeleteExpert(String id)
        {
            Repository.DeleteObject<A4AExpert>(id);
            return ListExpert();
        }
        #endregion EXPERT


        #region PROFESSION
        public ViewModelListBase ListProfession()
        {
            return new ViewModelListBase(typeof(A4AProfessionDetailViewModel), Repository
                    .QueryObjects<A4AProfession>()
                    .Select(x => new A4AProfessionDetailViewModel(x)));
        }

        public A4AProfessionDetailViewModel NewProfession()
        {
            return new A4AProfessionDetailViewModel(new A4AProfession()).AddForeignKeys<A4AProfessionDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4AProfession>());
        }

        public A4AProfessionDetailViewModel EditProfession(string id)
        {
            return new A4AProfessionDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4AProfession>(id))
                .AddForeignKeys<A4AProfessionDetailViewModel>(Repository.GetPossibleForeignKeys<A4AProfession>());
        }

        public ViewModelListBase SaveProfession(IFormCollection form)
        {
            var Profession = Repository.SaveObject(new A4AProfessionDetailViewModel(form).ModelClassFromViewModel());
            return ListProfession();

        }

        public ViewModelListBase DeleteProfession(String id)
        {
            Repository.DeleteObject<A4AProfession>(id);
            return ListProfession();
        }
        #endregion PROFESSION


        #region CATEGORY
        public ViewModelListBase ListCategory()
        {
            return new ViewModelListBase(typeof(A4ACategoryDetailViewModel), Repository
                    .QueryObjects<A4ACategory>()
                    .Select(x => new A4ACategoryDetailViewModel(x)));
        }

        public A4ACategoryDetailViewModel NewCategory()
        {
            return new A4ACategoryDetailViewModel(new A4ACategory()).AddForeignKeys<A4ACategoryDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4ACategory>());
        }

        public A4ACategoryDetailViewModel EditCategory(string id)
        {
            return new A4ACategoryDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4ACategory>(id))
                .AddForeignKeys<A4ACategoryDetailViewModel>(Repository.GetPossibleForeignKeys<A4ACategory>());
        }

        public ViewModelListBase SaveCategory(IFormCollection form)
        {
            var Category = Repository.SaveObject(new A4ACategoryDetailViewModel(form).ModelClassFromViewModel());
            return ListCategory();

        }

        public ViewModelListBase DeleteCategory(String id)
        {
            Repository.DeleteObject<A4ACategory>(id);
            return ListCategory();
        }
        #endregion CATEGORY 

        #region SUBCATEGORY
        public ViewModelListBase ListSubCategory()
        {
            return new ViewModelListBase(typeof(A4ASubCategoryDetailViewModel), Repository
                    .QueryObjects<A4ASubCategory>()
                    .Select(x => new A4ASubCategoryDetailViewModel(x)));
        }

        public A4ASubCategoryDetailViewModel NewSubCategory()
        {
            return new A4ASubCategoryDetailViewModel(new A4ASubCategory()).AddForeignKeys<A4ASubCategoryDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4ASubCategory>());
        }

        public A4ASubCategoryDetailViewModel EditSubCategory(string id)
        {
            return new A4ASubCategoryDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4ASubCategory>(id))
                .AddForeignKeys<A4ASubCategoryDetailViewModel>(Repository.GetPossibleForeignKeys<A4ASubCategory>());
        }

        public ViewModelListBase SaveSubCategory(IFormCollection form)
        {
            var SubCategory = Repository.SaveObject(new A4ASubCategoryDetailViewModel(form).ModelClassFromViewModel());
            return ListSubCategory();

        }

        public ViewModelListBase DeleteSubCategory(String id)
        {
            Repository.DeleteObject<A4ASubCategory>(id);
            return ListSubCategory();
        }
        #endregion SUBCATEGORY 

        #region Location
        public ViewModelListBase ListLocation()
        {
            return new ViewModelListBase(typeof(A4ALocationDetailViewModel), Repository
                    .QueryObjects<A4ALocation>()
                    .Select(x => new A4ALocationDetailViewModel(x)));
        }

        public A4ALocationDetailViewModel NewLocation()
        {
            return new A4ALocationDetailViewModel(new A4ALocation()).AddForeignKeys<A4ALocationDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4ALocation>());
        }

        public A4ALocationDetailViewModel EditLocation(string id)
        {
            return new A4ALocationDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4ALocation>(id))
                .AddForeignKeys<A4ALocationDetailViewModel>(Repository.GetPossibleForeignKeys<A4ALocation>());
        }

        public ViewModelListBase SaveLocation(IFormCollection form)
        {
            var Location = Repository.SaveObject(new A4ALocationDetailViewModel(form).ModelClassFromViewModel());
            return ListLocation();

        }

        public ViewModelListBase DeleteLocation(String id)
        {
            Repository.DeleteObject<A4ALocation>(id);
            return ListLocation();
        }
        #endregion Location 



        #region SUBSCRIPTION
        public ViewModelListBase ListSubscription()
        {
            return new ViewModelListBase(typeof(A4ASubscriptionDetailViewModel), Repository
                    .QueryObjects<A4ASubscription>()
                    .Select(x => new A4ASubscriptionDetailViewModel(x)));
        }

        public A4ASubscriptionDetailViewModel NewSubscription()
        {
            return new A4ASubscriptionDetailViewModel(new A4ASubscription()).AddForeignKeys<A4ASubscriptionDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4ASubscription>());
        }

        public A4ASubscriptionDetailViewModel EditSubscription(string id)
        {
            return new A4ASubscriptionDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4ASubscription>(id))
                .AddForeignKeys<A4ASubscriptionDetailViewModel>(Repository.GetPossibleForeignKeys<A4ASubscription>());
        }

        public ViewModelListBase SaveSubscription(IFormCollection form)
        {
            var Subscription = Repository.SaveObject(new A4ASubscriptionDetailViewModel(form).ModelClassFromViewModel());
            return ListSubscription();

        }

        public ViewModelListBase DeleteSubscription(String id)
        {
            Repository.DeleteObject<A4ASubscription>(id);
            return ListSubscription();
        }
        #endregion SUBCATEGORY 

        #region ADMINISTRATORS
        public ViewModelListBase ListAdministrator()
        {
            return new ViewModelListBase(typeof(A4AAdministratorDetailViewModel), Repository
                    .QueryObjects<A4AAdministrator>()
                    .Select(x => new A4AAdministratorDetailViewModel(x)));
        }

        public A4AAdministratorDetailViewModel NewAdministrator()
        {
            return new A4AAdministratorDetailViewModel(new A4AAdministrator()).AddForeignKeys<A4AAdministratorDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4AAdministrator>());
        }

        public A4AAdministratorDetailViewModel EditAdministrator(string id)
        {
            return new A4AAdministratorDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4AAdministrator>(id))
                .AddForeignKeys<A4AAdministratorDetailViewModel>(Repository.GetPossibleForeignKeys<A4AAdministrator>());
        }

        public ViewModelListBase SaveAdministrator(IFormCollection form)
        {
            var Administrator = Repository.SaveObject(new A4AAdministratorDetailViewModel(form).ModelClassFromViewModel());
            return ListAdministrator();

        }

        public ViewModelListBase DeleteAdministrator(String id)
        {
            Repository.DeleteObject<A4AAdministrator>(id);
            return ListAdministrator();
        }
        #endregion ADMINISTRATORS 

        #region USERS
        public ViewModelListBase ListUser()
        {
            return new ViewModelListBase(typeof(A4AUserDetailViewModel), Repository
                    .QueryObjects<A4AUser>()
                    .Select(x => new A4AUserDetailViewModel(x, ModelNames.Verb.List)));
        }

        public A4AUserDetailViewModel NewUser()
        {
            return new A4AUserDetailViewModel(new A4AUser(), ModelNames.Verb.New).AddForeignKeys<A4AUserDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4AUser>());
        }

        public A4AUserDetailViewModel EditUser(string id)
        {
            return new A4AUserDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4AUser>(id),
                    ModelNames.Verb.Edit)
                .AddForeignKeys<A4AUserDetailViewModel>(Repository.GetPossibleForeignKeys<A4AUser>());
        }

        public ViewModelListBase SaveUser(IFormCollection form)
        {
            var User = Repository.SaveObject(new A4AUserDetailViewModel(form).ModelClassFromViewModel());
            return ListUser();

        }

        public ViewModelListBase DeleteUser(String id)
        {
            Repository.DeleteObject<A4AUser>(id);
            return ListUser();
        }
        #endregion USERS 

        
        #region MESSAGES
        public ViewModelListBase ListMessage(A4AMailboxType listType)
        {
            return new ViewModelListBase(typeof(A4AMessageSummaryViewModel), Repository
                    .QueryObjects<A4AMessage>()
                    .Select(x => new A4AMessageSummaryViewModel(x)));
        }

        public A4AMessageDetailViewModel NewMessage()
        {
            return new A4AMessageDetailViewModel(new A4AMessage()).AddForeignKeys<A4AMessageDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4AMessage>());
        }

        public A4AMessageDetailViewModel EditMessage(string id)
        {
            return new A4AMessageDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4AMessage>(id))
                .AddForeignKeys<A4AMessageDetailViewModel>(Repository.GetPossibleForeignKeys<A4AMessage>());
        }

        //$"A4A Question on '{msg.Topic}'

        public ViewModelListBase SaveMessage(IFormCollection form)
        {
            var mail = new A4AMessageDetailViewModel(form).ModelClassFromViewModel();

            mail.Subject = $"A4A Question on '{mail.Topic}'";

            mail.Date = DateTime.Now.ToUniversalTime().ToString("r");
           
            var userAndExperts = Repository.GetUserAndExpertsForMessage(mail);

            if (!userAndExperts.experts.Any())
                throw new Exception($"Message topic did not select any experts - not sending or saving ... ");

            mail = Repository.AddObject(mail);

            foreach (var record in sender.SendMail(GetEmailDefinition(),mail, userAndExperts.user, userAndExperts.experts))
            {
                var newEmailRecord = Repository.AddObject(record);
            }

            return ListMessage(A4AMailboxType.Inbox);

        }

        public ViewModelListBase DeleteMessage(String id)
        {
            Repository.DeleteObject<A4AMessage>(id);
            return ListMessage(A4AMailboxType.Inbox);
        }
        #endregion USERS 

        #region EMAILRECORD

        public ViewModelListBase ListEmailRecord(A4AMailboxType listtype)
        {
            return new ViewModelListBase(typeof(A4AEmailRecordSummaryViewModel), Repository
                .QueryObjects<A4AEmailRecord>()
                .Select(x => new A4AEmailRecordSummaryViewModel(x)));
        }

        public A4AEmailRecordDetailViewModel EditEmailRecord(string id)
        {
            return new A4AEmailRecordDetailViewModel(
                Repository.GetObjectByPrimaryKey<A4AEmailRecord>(id)).
                AddForeignKeys<A4AEmailRecordDetailViewModel>(Repository.GetPossibleForeignKeys<A4AEmailRecord>());
        }

        #endregion



        public void PollEmailState(Dictionary<string,DateTime> processedEvents)
        {
            var emailDefinition = GetEmailDefinition();

            // don't keep track of any events that are before our purge time. 

            var purgeTime = DateTime.Now.ToUniversalTime().AddMilliseconds(-emailDefinition.LookbackMilliseconds);
            foreach (var c in processedEvents.ToArray())
            {
                if (c.Value < purgeTime)
                    processedEvents.Remove(c.Key);
            }

            var emailEvents = sender.GetNextMailEvents(emailDefinition);

            SaveEmailDefinition(emailDefinition);

            if (emailEvents?.items == null)
            {
                L.Trace("Found null email events or email events.Items ...");
                return;
            }

            var incomingEmails = new List<A4AEmailRecord>(); 
            foreach (var c in emailEvents.items)
            {
                if (processedEvents.ContainsKey(c.id))
                    continue;

                if (c.message.headers.from.Contains(emailDefinition.Domain))
                {
                    if (c.eventType == EventTypes.delivered)
                    {
                        try
                        {
                            var emailRecord =
                                Repository.UpdateEmailRecordStatus(c.message.headers.messageid, c.eventType.ToString());

                            L.Trace($"Updated email record - {emailRecord.ToJSonString()}");

                           
                        }
                        catch (Exception e)
                        {
                            L.Trace($"An exception - {e.Message} - occured processign email event - {c.ToJSonString()}");
                        }

                        processedEvents[c.id] = DateTime.Now.ToUniversalTime();
                    }
                }
                else
                {
                    // a message to us - a reply - figure out who it is from and to
                    try
                    {
                        var fromEmailAndUser = c.message.headers.from.ParseLongEmailString();
                        var toEmailAndUser = c.message.headers.to.ParseLongEmailString();

                        var correspondents =
                            Repository.GetUserAndExpertForReply(toEmailAndUser.userPrefix, $"{fromEmailAndUser.userPrefix}@{fromEmailAndUser.domain}");

                        var emailRecord = new A4AEmailRecord
                        {
                            EmailFrom = correspondents.expert.Email,
                            EmailTo = correspondents.user.Email,
                            NameFrom = correspondents.expert.ExpertName,
                            NameTo = correspondents.user.UserName,
                            ExternalMessageId = c.message.headers.messageid,
                            ExternalStatus = c.eventType.ToString(),
                            Url = c.storage.url,
                            Subject = c.message.headers.subject
                        };

                        incomingEmails.Add(emailRecord);

                    }
                    catch (Exception e)
                    {
                        L.Trace($"An exception - {e.Message} - occured processign email event - {c.ToJSonString()}");
                    }

                    processedEvents[c.id] = DateTime.Now.ToUniversalTime();
                }
            }

            foreach (var c in incomingEmails)
            {
                try
                {
                    var emailPostResponse = sender.EmailFromUrl(emailDefinition, c.Url);

                    var message = new A4AMessage
                    {
                        Content = emailPostResponse.BodyPlain,
                        Subject = emailPostResponse.Subject,
                        Date = emailPostResponse.Date,
                        EmailSender = c.EmailFrom
                    };

                    message = Repository.AddObject(message);

                    c.MessageId = message.MessageId;

                    Repository.AddObject(c);
                }
                catch (Exception e)
                {
                    L.Trace($"An exception - {e.Message} - occured processing incoming email event - {c.ToJSonString()}");

                }
            }

        }


        public GraphResponse RunQuery(GraphQuery query)
        {
            return Repository.RunQuery(query);
        }
    }
}