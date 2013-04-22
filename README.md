karmencita
==========

Karmencita is a object query language for .NET. It was written before LINQ when the night was still dark and full of terrors.

Features:
- easy, SQL like language.
- common, slim API used for querying data.
- supports any IEnumerable data source, DataTables and XmlDataDocuments.
- extensible implementation
- common API but still get results depending on the data source. (for instance when querying XmlDataDocuments we get back XmlElement[]. But if we query a DataTable we get back a DataRow[]).
- supports IComparable for custom type implementation.

Here are a few quick samples:

- query a array of System.Diagnostics.Process objects
![](http://voidsoft.ro/content/image/karmencita_code.png)

- query a DataTable (the Northwind database products table)
![](http://voidsoft.ro/content/image/karmencita_code_2.png)

- query a XmlDataDocument
![](http://voidsoft.ro/content/image/karmencita_code_3.png)
