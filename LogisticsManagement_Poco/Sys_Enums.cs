using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_Poco
{
    public enum OrderType
    {
        DeliveryOrder = 1,
        ReturnOrder,
        MiscellaneousOrder,
    }

    public enum Enum_TaxToCall
    {
        GST = 1,
        HST,
        PST,
        QST,
        TAX,
        VAT
    }

    public enum Enum_AccountType
    {
        Asset = 1,
        Liability,
        Income,
        Expense
    }
    public enum Enum_EmployeeType
    {
        Employee = 1,
        EmployeeDriver,
        Salesman,
        Agent,
        Broker,
        OwnerOperator,
        UnknownType,
    }

    public enum Enum_UserGroup
    {
        Administrator = 1,
        Manager,
        Supervisor,
        Restricted,
        ThirdParty
    }

    public enum Enum_AddressType
    {
        Billing = 1,
        Shipping,
        Residential,
        Warehouse
    }

}
