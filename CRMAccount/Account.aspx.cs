using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm;
using Microsoft.Crm.Sdk.Messages;

namespace CRMAccount
{
    public partial class Account : System.Web.UI.Page
    {
        IOrganizationService service;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                if (Page.Request.QueryString["id"] != null)
                {
                    string guid = Page.Request.QueryString["id"].Substring(0, 36);
                    Guid accountId = new Guid(guid);

                    string UserName = "x@x.onmicrosoft.com";
                    string Password = "xxxx";

                    service = null;
                    ClientCredentials clientCredentials = new ClientCredentials();
                    clientCredentials.UserName.UserName = UserName;
                    clientCredentials.UserName.Password = Password;
                    ClientCredentials deviceCredentials = null;
                    Uri OrganizationUri = new Uri("https://xxxxxx.api.crm4.dynamics.com/XRMServices/2011/Organization.svc");
                    Uri HomeRealmUri = null;
                    OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(OrganizationUri, HomeRealmUri, clientCredentials, deviceCredentials);
                    service = (IOrganizationService)serviceProxy;
                
                    Entity result = service.Retrieve("account", accountId, new ColumnSet(new string[] { "name", "accountid" }));
                    TreeNode tnRoot = new TreeNode();
                    tnRoot.Value = result.Attributes["name"].ToString();
                    tnRoot.SelectAction = TreeNodeSelectAction.Expand;
                    List<TreeNode> rootNodes = new List<TreeNode>();
                    rootNodes = GetChildNodes(accountId);
                    foreach (TreeNode account in rootNodes)
                    {
                        tnRoot.ChildNodes.Add(account);
                    }
                    tvAccount.Nodes.Add(tnRoot);
 
                }
            }
            catch (Exception ex)
            {
                Page.Response.Write(ex.ToString());
            }
        }

        private List<TreeNode> GetChildNodes(Guid accountId)
        {
            try
            {
                List<TreeNode> nodes = new List<TreeNode>();
                ConditionExpression condition = new ConditionExpression("parentaccountid", ConditionOperator.Equal, accountId);
                FilterExpression filter = new FilterExpression();
                filter.Conditions.Add(condition);
                QueryExpression query = new QueryExpression();
                query.EntityName = "account";
                query.ColumnSet = new ColumnSet(new string[] { "name", "accountid" });
                query.Criteria = filter;
                EntityCollection result = service.RetrieveMultiple(query);

                foreach (Entity account in result.Entities)
                {
                    TreeNode tn = new TreeNode();
                    tn.Value = account.Attributes["name"].ToString();
                    tn.SelectAction = TreeNodeSelectAction.Expand;
                    Guid accountChildId = account.Id;
                    List<TreeNode> nodesChild = GetChildNodes(accountChildId);
                    foreach (TreeNode nodeChild in nodesChild)
                    {
                        tn.ChildNodes.Add(nodeChild);
                    }
                    nodes.Add(tn);
                }
                return nodes;
            }
            catch (Exception ex)
            {
                return new List<TreeNode>();
            }
        }

    }
}