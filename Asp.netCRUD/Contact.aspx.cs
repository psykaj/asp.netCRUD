using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace Asp.netCRUD
{
    public partial class Contact1 : System.Web.UI.Page
    {
        //string CS = "data source = .; databsae = aspCRUD ; integrated security = SSPI";
        SqlConnection sqlCon = new SqlConnection(@"data source = DESKTOP-P8FARIT\SQLEXPRESS; database = aspCRUD ; integrated security = true");
        //SqlCommand cmd = new SqlCommand("select * from contact" , sqlCon);
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                btnDelete.Enabled = false;
                fillGridView();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
        }

        public void clear()
        {
            hfContactID.Value = "";
            txtName.Text = txtMobile.Text = txtAddress.Text = "";
            lblSuccessMsg.Text = lblErrorMsg.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if(sqlCon.State == System.Data.ConnectionState.Closed)
            {
                sqlCon.Open();
            }

            SqlCommand sqlCmd = new SqlCommand("ContactCreateOrUpdate",sqlCon);
            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@contactId",(hfContactID.Value == "" ? 0 : Convert.ToInt32(hfContactID.Value)));
            sqlCmd.Parameters.AddWithValue("@personName",txtName.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@mobile",txtMobile.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@address",txtAddress.Text.Trim());
            sqlCmd.ExecuteNonQuery();
            sqlCon.Close();
            string contactId = hfContactID.Value;
            clear();

            if(contactId == "")
            {
                lblSuccessMsg.Text = "Saved Successfully";
            }
            else
            {
                lblSuccessMsg.Text = "Updated Successfully";
            }
            fillGridView();
        }

        void fillGridView()
        {
            if (sqlCon.State == System.Data.ConnectionState.Closed)
            {
                sqlCon.Open();
            }

            SqlDataAdapter sqlDa = new SqlDataAdapter("contactViewAll", sqlCon);
            sqlDa.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            DataTable dtbl = new DataTable();
            sqlDa.Fill(dtbl);
            sqlCon.Close();
            gvContact.DataSource = dtbl;
            gvContact.DataBind();
        }

        protected void lnkOnclick(object sender,EventArgs e)
        {
            int contactId = Convert.ToInt32((sender as LinkButton).CommandArgument);
            SqlDataAdapter sqlDa = new SqlDataAdapter("contactViewById", sqlCon);
            sqlDa.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlDa.SelectCommand.Parameters.AddWithValue("@contactId", contactId);
            DataTable dtbl = new DataTable();
            sqlDa.Fill(dtbl);
            sqlCon.Close();
            hfContactID.Value = contactId.ToString();
            txtName.Text = dtbl.Rows[0]["personName"].ToString();
            txtMobile.Text = dtbl.Rows[0]["Mobile"].ToString();
            txtAddress.Text = dtbl.Rows[0]["Address"].ToString();
            btnSave.Text = "Update";
            btnDelete.Enabled = true;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (sqlCon.State == System.Data.ConnectionState.Closed)
            {
                sqlCon.Open();
            }

            SqlCommand sqlCmd = new SqlCommand("contactDeleteById", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@contactId", Convert.ToInt32(hfContactID.Value));
            sqlCmd.ExecuteNonQuery();
            sqlCon.Close();
            clear();
            fillGridView();
            lblSuccessMsg.Text = "Deleted Successfully";
        }
    }
}