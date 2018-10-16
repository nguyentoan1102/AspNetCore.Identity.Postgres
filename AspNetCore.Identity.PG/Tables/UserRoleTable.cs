using System;
using System.Collections.Generic;
using AspNetCore.Identity.PG.Context;

namespace AspNetCore.Identity.PG.Tables
{
    /// <summary>
    /// Classe que representa a tabela AspNetUserRoles na base PostgreSQL
    /// </summary>
    public class UserRolesTable
    {
        private readonly PostgreSQLDatabase _database;

        internal const string tableName = "AspNetUserRoles";
        internal const string fieldUserID = "UserId";
        internal const string fieldRoleID = "RoleId";
        internal static string fullTableName = Consts.Schema.Quote() + "." + tableName.Quote();
        

        /// <summary>
        /// Constructor that takes a PostgreSQLDatabase instance.
        /// </summary>
        /// <param name="database"></param>
        public UserRolesTable(PostgreSQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Retorna uma lista de Roles do Usuário em questão
        /// </summary>
        /// <param name="userId">Código do Usuário</param>
        /// <returns></returns>
        public List<string> FindByUserId(Guid userId)
        {
            List<string> roles = new List<string>();
            //TODO: This probably does not work, and may need testing.

            string commandText = "select AspRoles." + RoleTable<IdentityRole>.fieldName.Quote() + " from " + UserTable<IdentityUser>.fullTableName + " AspUsers" +
                " INNER JOIN " + fullTableName + " AspUserRoles " +
                " ON AspUsers." + UserTable<IdentityUser>.FieldId.Quote() + " = AspUserRoles." + fieldUserID.Quote() +
                " INNER JOIN " + RoleTable<IdentityRole>.fullTableName + " AspRoles " +
                " ON AspUserRoles." + fieldRoleID.Quote() + " = AspRoles." + RoleTable<IdentityRole>.fieldId.Quote() +
                " where AspUsers." + UserTable<IdentityUser>.FieldId.Quote() + " = @userId";
                        
            /*select AspNetRoles.Name from AspNetUsers
             * inner join AspNetUserRoles
             * ON AspNetUsers.ID = AspNetUserRoles.UserID
             * inner join AspNetRoles
             * ON aspNetUserRoles.RoleID = AspNetRoles.ID
             * where AspNetUser.ID = :id
            */

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            var rows = _database.ExecuteQuery(commandText, parameters);
            foreach(var row in rows)
            {
                roles.Add(row[RoleTable<IdentityRole>.fieldName]);
            }

            return roles;
        }

        /// <summary>
        /// Deletes all roles from a user in the AspNetUserRoles table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "DELETE FROM "+fullTableName+" WHERE "+fieldUserID.Quote()+" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);

            return _database.ExecuteSQL(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new role record for a user in the UserRoles table.
        /// </summary>
        /// <param name="ur"></param>
        /// <returns></returns>

        internal int Insert(IdentityUserRole ur)
        {
            string commandText = "INSERT INTO " + fullTableName + " (" + fieldUserID.Quote() + ", " + fieldRoleID.Quote() + ") VALUES (@userId, @roleId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", ur.UserId);
            parameters.Add("roleId", ur.RoleId.ToString());

            return _database.ExecuteSQL(commandText, parameters);
        }

        internal int Delete(Guid roleId, Guid userId)
        {
            string commandText = "DELETE FROM " + fullTableName + " WHERE " + fieldUserID.Quote() + " = @userId and "+fieldRoleID.Quote()+" = @roleId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);
            parameters.Add("roleId", roleId.ToString());

            return _database.ExecuteSQL(commandText, parameters);
        }

        public bool GetRoleExistsInUser(Guid userId, Guid roleId)
        {
            string commandText = "SELECT * from " + fullTableName + " " +
                                 "WHERE " + fieldUserID.Quote() + " = @userId and " + fieldRoleID.Quote() +
                                 " = @roleId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);
            parameters.Add("roleId", roleId.ToString());

            return _database.ExecuteQuery(commandText, parameters).Count > 0;
        }
    }
}
