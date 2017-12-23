using System;
using System.Collections.Generic;
using System.Linq;
using As.A4ACore;
using As.GraphDB.Sql;
using Microsoft.AspNetCore.Http;

namespace App4Answers.Models.A4Amodels
{
    public class A4AModel1
    {
        public A4ARepository Repository { get; private set; }

        public ObjectTypesAndVerbs.ObjectType ObjectType { get; set; }
        public ObjectTypesAndVerbs.Verb Verb { get; set; }

        public A4AModel1(A4ARepository repository)
        {
            this.Repository = repository;
        }
        #region COMPANY


        public A4ACompanyDetailViewModel NewCompany()
        {
            return new A4ACompanyDetailViewModel(new A4ACompany(), ObjectTypesAndVerbs.Verb.New)
                .AddForeignKeys<A4ACompanyDetailViewModel>(Repository.GetPossibleForeignKeys<A4ACompany>());
        }

        public ViewModelListBase ListCompany()
        {
            return new ViewModelListBase(typeof(A4ACompanySummaryViewModel), Repository
                .QueryObjects<A4ACompany>($"", new Range(), new Sort())
                .Select(x => new A4ACompanySummaryViewModel(x,ObjectTypesAndVerbs.Verb.List)),
                ObjectTypesAndVerbs.ObjectType.Company,ObjectTypesAndVerbs.Verb.List);
        }

        public A4ACompanyDetailViewModel EditCompany(string id)
        {
            return new A4ACompanyDetailViewModel(Repository.GetObjectByPrimaryKey<A4ACompany>(id),
                    ObjectTypesAndVerbs.Verb.Edit)
                .AddForeignKeys<A4ACompanyDetailViewModel>(Repository.GetPossibleForeignKeys<A4ACompany>());
        }

        public ViewModelListBase SaveCompany(IFormCollection form)
        {
            var party = Repository.SaveObject(new A4ACompanyDetailViewModel(form).ModelClassFromViewModel());
            return ListCompany();
        }

        public void DeleteCompany(String id)
        {
            Repository.DeleteObject<A4ACompany>(id);
        }
        #endregion

        #region EXPERT
        public ViewModelListBase ListExpert()
        {
            return new ViewModelListBase(typeof(A4AExpertSummaryViewModel), Repository
                    .QueryObjects<A4AExpert>($"", new Range(), new Sort())
                    .Select(x => new A4AExpertSummaryViewModel(x, ObjectTypesAndVerbs.Verb.List)),
                ObjectTypesAndVerbs.ObjectType.Expert, ObjectTypesAndVerbs.Verb.List);
        }

        public A4AExpertDetailViewModel NewExpert()
        {
            return new A4AExpertDetailViewModel(new A4AExpert(), ObjectTypesAndVerbs.Verb.New).AddForeignKeys<A4AExpertDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4AExpert>());
        }

        public A4AExpertDetailViewModel EditExpert(string id)
        {
            return new A4AExpertDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4AExpert>(id),
                    ObjectTypesAndVerbs.Verb.Edit)
                .AddForeignKeys<A4AExpertDetailViewModel>(Repository.GetPossibleForeignKeys<A4AExpert>());
        }

        public ViewModelListBase SaveExpert(IFormCollection form)
        {
            var expert = Repository.SaveObject(new A4AExpertDetailViewModel(form).ModelClassFromViewModel());
            return ListExpert();

        }

        public void DeleteExpert(String id)
        {
            Repository.DeleteObject<A4AExpert>(id);
        }
        #endregion EXPERT


        #region PROFESSION
        public ViewModelListBase ListProfession()
        {
            return new ViewModelListBase(typeof(A4AProfessionDetailViewModel), Repository
                    .QueryObjects<A4AProfession>($"", new Range(), new Sort())
                    .Select(x => new A4AProfessionDetailViewModel(x, ObjectTypesAndVerbs.Verb.List)),
                ObjectTypesAndVerbs.ObjectType.Profession, ObjectTypesAndVerbs.Verb.List);
        }

        public A4AProfessionDetailViewModel NewProfession()
        {
            return new A4AProfessionDetailViewModel(new A4AProfession(), ObjectTypesAndVerbs.Verb.New).AddForeignKeys<A4AProfessionDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4AProfession>());
        }

        public A4AProfessionDetailViewModel EditProfession(string id)
        {
            return new A4AProfessionDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4AProfession>(id),
                    ObjectTypesAndVerbs.Verb.Edit)
                .AddForeignKeys<A4AProfessionDetailViewModel>(Repository.GetPossibleForeignKeys<A4AProfession>());
        }

        public ViewModelListBase SaveProfession(IFormCollection form)
        {
            var Profession = Repository.SaveObject(new A4AProfessionDetailViewModel(form).ModelClassFromViewModel());
            return ListProfession();

        }

        public void DeleteProfession(String id)
        {
            Repository.DeleteObject<A4AProfession>(id);
        }
        #endregion PROFESSION


        #region CATEGORY
        public ViewModelListBase ListCategory()
        {
            return new ViewModelListBase(typeof(A4ACategoryDetailViewModel), Repository
                    .QueryObjects<A4ACategory>($"", new Range(), new Sort())
                    .Select(x => new A4ACategoryDetailViewModel(x, ObjectTypesAndVerbs.Verb.List)),
                ObjectTypesAndVerbs.ObjectType.Category, ObjectTypesAndVerbs.Verb.List);
        }

        public A4ACategoryDetailViewModel NewCategory()
        {
            return new A4ACategoryDetailViewModel(new A4ACategory(), ObjectTypesAndVerbs.Verb.New).AddForeignKeys<A4ACategoryDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4ACategory>());
        }

        public A4ACategoryDetailViewModel EditCategory(string id)
        {
            return new A4ACategoryDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4ACategory>(id),
                    ObjectTypesAndVerbs.Verb.Edit)
                .AddForeignKeys<A4ACategoryDetailViewModel>(Repository.GetPossibleForeignKeys<A4ACategory>());
        }

        public ViewModelListBase SaveCategory(IFormCollection form)
        {
            var Category = Repository.SaveObject(new A4ACategoryDetailViewModel(form).ModelClassFromViewModel());
            return ListCategory();

        }

        public void DeleteCategory(String id)
        {
            Repository.DeleteObject<A4ACategory>(id);
        }
        #endregion CATEGORY 

        #region SUBCATEGORY
        public ViewModelListBase ListSubCategory()
        {
            return new ViewModelListBase(typeof(A4ASubCategoryDetailViewModel), Repository
                    .QueryObjects<A4ASubCategory>($"", new Range(), new Sort())
                    .Select(x => new A4ASubCategoryDetailViewModel(x, ObjectTypesAndVerbs.Verb.List)),
                ObjectTypesAndVerbs.ObjectType.SubCategory, ObjectTypesAndVerbs.Verb.List);
        }

        public A4ASubCategoryDetailViewModel NewSubCategory()
        {
            return new A4ASubCategoryDetailViewModel(new A4ASubCategory(), ObjectTypesAndVerbs.Verb.New).AddForeignKeys<A4ASubCategoryDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4ASubCategory>());
        }

        public A4ASubCategoryDetailViewModel EditSubCategory(string id)
        {
            return new A4ASubCategoryDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4ASubCategory>(id),
                    ObjectTypesAndVerbs.Verb.Edit)
                .AddForeignKeys<A4ASubCategoryDetailViewModel>(Repository.GetPossibleForeignKeys<A4ASubCategory>());
        }

        public ViewModelListBase SaveSubCategory(IFormCollection form)
        {
            var SubCategory = Repository.SaveObject(new A4ASubCategoryDetailViewModel(form).ModelClassFromViewModel());
            return ListSubCategory();

        }

        public void DeleteSubCategory(String id)
        {
            Repository.DeleteObject<A4ASubCategory>(id);
        }
        #endregion SUBCATEGORY 

        #region SUBSCRIPTION
        public ViewModelListBase ListSubscription()
        {
            return new ViewModelListBase(typeof(A4ASubscriptionDetailViewModel), Repository
                    .QueryObjects<A4ASubscription>($"", new Range(), new Sort())
                    .Select(x => new A4ASubscriptionDetailViewModel(x, ObjectTypesAndVerbs.Verb.List)),
                ObjectTypesAndVerbs.ObjectType.Subscription, ObjectTypesAndVerbs.Verb.List);
        }

        public A4ASubscriptionDetailViewModel NewSubscription()
        {
            return new A4ASubscriptionDetailViewModel(new A4ASubscription(), ObjectTypesAndVerbs.Verb.New).AddForeignKeys<A4ASubscriptionDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4ASubscription>());
        }

        public A4ASubscriptionDetailViewModel EditSubscription(string id)
        {
            return new A4ASubscriptionDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4ASubscription>(id),
                    ObjectTypesAndVerbs.Verb.Edit)
                .AddForeignKeys<A4ASubscriptionDetailViewModel>(Repository.GetPossibleForeignKeys<A4ASubscription>());
        }

        public ViewModelListBase SaveSubscription(IFormCollection form)
        {
            var Subscription = Repository.SaveObject(new A4ASubscriptionDetailViewModel(form).ModelClassFromViewModel());
            return ListSubscription();

        }

        public void DeleteSubscription(String id)
        {
            Repository.DeleteObject<A4ASubCategory>(id);
        }
        #endregion SUBCATEGORY 

        #region ADMINISTRATORS
        public ViewModelListBase ListAdministrator()
        {
            return new ViewModelListBase(typeof(A4AAdministratorDetailViewModel), Repository
                    .QueryObjects<A4AAdministrator>($"", new Range(), new Sort())
                    .Select(x => new A4AAdministratorDetailViewModel(x, ObjectTypesAndVerbs.Verb.List)),
                ObjectTypesAndVerbs.ObjectType.Administrator, ObjectTypesAndVerbs.Verb.List);
        }

        public A4AAdministratorDetailViewModel NewAdministrator()
        {
            return new A4AAdministratorDetailViewModel(new A4AAdministrator(), ObjectTypesAndVerbs.Verb.New).AddForeignKeys<A4AAdministratorDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4AAdministrator>());
        }

        public A4AAdministratorDetailViewModel EditAdministrator(string id)
        {
            return new A4AAdministratorDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4AAdministrator>(id),
                    ObjectTypesAndVerbs.Verb.Edit)
                .AddForeignKeys<A4AAdministratorDetailViewModel>(Repository.GetPossibleForeignKeys<A4AAdministrator>());
        }

        public ViewModelListBase SaveAdministrator(IFormCollection form)
        {
            var Administrator = Repository.SaveObject(new A4AAdministratorDetailViewModel(form).ModelClassFromViewModel());
            return ListAdministrator();

        }

        public void DeleteAdministrator(String id)
        {
            Repository.DeleteObject<A4ASubCategory>(id);
        }
        #endregion ADMINISTRATORS 

        #region USERS
        public ViewModelListBase ListUser()
        {
            return new ViewModelListBase(typeof(A4AUserDetailViewModel), Repository
                    .QueryObjects<A4AUser>($"", new Range(), new Sort())
                    .Select(x => new A4AUserDetailViewModel(x, ObjectTypesAndVerbs.Verb.List)),
                ObjectTypesAndVerbs.ObjectType.User, ObjectTypesAndVerbs.Verb.List);
        }

        public A4AUserDetailViewModel NewUser()
        {
            return new A4AUserDetailViewModel(new A4AUser(), ObjectTypesAndVerbs.Verb.New).AddForeignKeys<A4AUserDetailViewModel>(
                Repository.GetPossibleForeignKeys<A4AUser>());
        }

        public A4AUserDetailViewModel EditUser(string id)
        {
            return new A4AUserDetailViewModel(
                    Repository.GetObjectByPrimaryKey<A4AUser>(id),
                    ObjectTypesAndVerbs.Verb.Edit)
                .AddForeignKeys<A4AUserDetailViewModel>(Repository.GetPossibleForeignKeys<A4AUser>());
        }

        public ViewModelListBase SaveUser(IFormCollection form)
        {
            var User = Repository.SaveObject(new A4AUserDetailViewModel(form).ModelClassFromViewModel());
            return ListUser();

        }

        public void DeleteUser(String id)
        {
            Repository.DeleteObject<A4ASubCategory>(id);
        }
        #endregion USERS 

        public GraphResponse RunQuery(GraphQuery query)
        {
            return Repository.RunQuery(query);
        }
    }
}