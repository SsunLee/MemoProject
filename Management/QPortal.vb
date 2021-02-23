
Imports System.Collections.Generic
Imports System.Data
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms
Imports Microsoft.WindowsAPICodePack.Dialogs
Imports OfficeOpenXml
Imports WinControls.ListView
Imports WinControls.ListView.Collections

Namespace Qportals
    Public Class QPortal

    End Class

    Public Class Level

        Public Class AddPjtInfo

            Private ListBox As ListBox
            Private ListBox2 As ListBox
            Property _r As Rectangle

            Public Sub PjtInfo(ByRef msg() As String, ByRef lst As ListBox, Optional ByRef initLsts As List(Of ListBox) = Nothing)
                Dim InputStep As String
                Dim r As Rectangle = _r
                Dim x = r.Left + (r.Width) / 5
                Dim y = r.Top + (r.Height) / 5
                InputStep = InputBox(msg(0), msg(1), msg(2), x, y)
                If Not (InputStep Is Nothing OrElse InputStep = "") Then
                    If lst.Items.Contains(InputStep) Then
                        Qportals.Debugging.Show(InputStep & "와 동일한 항목이 이미 존재합니다.", "Add", 0, 16)
                    Else
                        lst.Items.Add(InputStep)
                        lst.SelectedIndex = lst.Items.Count - 1

                        If Not (initLsts Is Nothing) Then
                            For Each li As ListBox In initLsts
                                li.Items.Clear()
                            Next
                        End If

                    End If
                End If
            End Sub

        End Class
        Public Class CommintCodes
            Private ListBox As ListBox
            Private Sub MakeQuery()
                Dim dbCon As DbConnection.Mysql_Class = New DbConnection.Mysql_Class
                Dim sql As String = Nothing

                sql = "Insert into td_defect.`Project` (Project, GroupName, Model, Step, StartDate, EndDate)"
                sql += " Values(zz,zz,zz,zz,zz,zz) on Duplicate key update StartDate= zz, EndDate = zz"
            End Sub
        End Class

        Public Class Member
            Private __lst As ListView
            Property _lst As ListView
                Get
                    Return __lst
                End Get
                Set(value As ListView)
                    __lst = value
                End Set
            End Property

            Public Sub MakeForm(ByRef Contain As Control, ByRef dt As DataTable)
                Dim addListView As Qportals.Controls.ListViewMaker = New Controls.ListViewMaker
                Dim form As Form = New Form With {
                    .Name = "Member", .Size = New Size(500, 500),
                    .Text = "인원을 선택 하세요."
                }
                _lst = New ListView With {.Name = "lst", .View = View.Details, .Size = New Size(400, 100)}
                With _lst.Columns
                    .Add("업체") : .Add("부서") : .Add("이름")
                    .Add("직급")
                End With
                addListView.BuildList(_lst, dt, New Integer() {0, 1, 2, 3})

                form.Controls.Add(_lst)
                form.ShowDialog()

            End Sub

        End Class

        Public Class SelectModeltoOtherForm
            ' T/C 할당할 때 모델 선택 하는 부분
            Public Sub New()

            End Sub

#Region "Get Set Property"
            '-------about Model--------
            Private __pro As String
            Private __grp As String
            Private __mod As String
            Private __step As String
            Property _pro As String
                Get
                    Return __pro
                End Get
                Set(value As String)
                    __pro = value
                End Set
            End Property
            Property _grp As String
                Get
                    Return __grp
                End Get
                Set(value As String)
                    __grp = value
                End Set
            End Property
            Property _mod As String
                Get
                    Return __mod
                End Get
                Set(value As String)
                    __mod = value
                End Set
            End Property
            Property _step As String
                Get
                    Return __step
                End Get
                Set(value As String)
                    __step = value
                End Set
            End Property

            '-------about DB----------
            Private __Connstring As String
            Private __SchemaString As String

            Property _Connstring As String
                Get
                    Return __Connstring
                End Get
                Set(value As String)
                    __Connstring = value
                End Set
            End Property
            Property _SchemaString As String
                Get
                    Return __SchemaString
                End Get
                Set(value As String)
                    __SchemaString = value
                End Set
            End Property
            Private _sql As String
            Property sql As String
                Get
                    Return _sql
                End Get
                Set(value As String)
                    _sql = value
                End Set
            End Property
            Private __dt As DataTable
            Property _dt As DataTable
                Get
                    Return __dt
                End Get
                Set(value As DataTable)
                    __dt = value
                End Set
            End Property

            '-----Controls------
            Private __listview As ListView
            Property _listview As ListView
                Get
                    Return __listview
                End Get
                Set(value As ListView)
                    __listview = value
                End Set
            End Property

#End Region
            '---- Error Check -----------
            Private __exitChk As Boolean
            Private Property _exitchk As Boolean
                Get
                    Return __exitChk
                End Get
                Set(value As Boolean)
                    __exitChk = value
                End Set
            End Property

            Public Sub BuildListMain()
                ImportProject_makeQuery()

                If (__exitChk = False) Then
                    Exit Sub
                End If

            End Sub

            '** Project 및 모델 정보를 가져오기 위해 쿼리를 작성 했습니다.
            Private Sub ImportProject_makeQuery()
                Dim chk As Boolean = False
                sql = Nothing
                chk = If(_Connstring Is Nothing And _SchemaString Is Nothing, False, True)

                If (chk) Then

                    Dim addP As String = If(_pro.ToUpper() = "", "", " And Project like '%" & Replace(_pro.ToUpper, "'", "''") & "%'")
                    Dim addG As String = If(_grp.ToUpper = "", "", " And GroupName = '" & Replace(_grp.ToUpper, "'", "''") & "'")
                    Dim addM As String = If(_mod.ToUpper = "", "", " And Model = '" & Replace(_mod.ToUpper, "'", "''") & "'")
                    Dim addS As String = If(_step.ToUpper = "", "", " And Step = '" & Replace(_step.ToUpper, "'", "''") & "'")
                    sql = "SELECT DISTINCT Project, GroupName, Model, Step, StartDate, EndDate FROM " & _SchemaString &
                        ".`project`" & " WHERE ID > 0 and " & "GroupName is not Null and Model is not null and Step is not null "
                    sql += addP & addG & addM & addS
                    sql += " order by Project, StartDate"

                Else
                    __exitChk = chk
                    Exit Sub
                End If

                ' Database Run
                ImportProject_RunDB()
            End Sub

            '** Mysql을 연결해서 Datatable로 자료를 가져 옵니다.
            Private Sub ImportProject_RunDB()
                Dim dbc As DbConnection.Mysql_Class = New DbConnection.Mysql_Class

                _dt = dbc.Mysql_to_datatable(sql, _Connstring & _SchemaString)

                __exitChk = If(Not (_dt Is Nothing), True, False)

            End Sub


        End Class

    End Class

    Public Class External_library

        Public Class EPPlus

            Public Sub datatable_to_excel(ByRef _dt As DataTable, ByRef fileName As String)
                Dim filedown As Qportals.FileControl = New FileControl
                Dim targetPath As String
                targetPath = filedown.Custom_folderselect
                Dim dt As System.Data.DataTable = New System.Data.DataTable
                Using p As New ExcelPackage()

                    Dim ws As ExcelWorksheet = p.Workbook.Worksheets.Add(fileName)
                    ws.Cells("A1").LoadFromDataTable(_dt, True)
                    targetPath = targetPath & "\"
                    p.SaveAs(New FileInfo(targetPath & fileName & ".xlsx"))
                    If targetPath <> "" Then
                        Diagnostics.Process.Start(IO.Path.GetDirectoryName(targetPath))
                    End If

                End Using

            End Sub



#Region "</Summary> Then 엑셀 열고 데이터를 Datatable로 가져오기  </Summary>"
            Public Function ReadAllxlsm_set_datatable(ByRef path As String, ByRef shtName As String) As DataTable
                '//  path : path is the file path
                '// shtName : shtName is the sheet name
                Dim chk As Boolean = False
                Dim returnDT As DataTable = Nothing
                Dim isExist As IO.FileInfo = New IO.FileInfo(path)
                Dim dt As System.Data.DataTable = New System.Data.DataTable
                Using p As New ExcelPackage(isExist)
                    Dim ms As ExcelWorksheet = p.Workbook.Worksheets(shtName)
                    If ms Is Nothing Then
                        Exit Function
                    End If

                    If (ms.Dimension Is Nothing) Then                 ' check if the sheet is completly empty
                        chk = True
                    End If

                    If (chk = True) Then
                        Return Nothing
                    End If

                    '----------------- Column -----------------------------
                    Dim columnNames As System.Collections.Generic.List(Of String) = New System.Collections.Generic.List(Of String)
                    ' needed to keep track of empty column headers
                    Dim currentColumn As Integer = 1
                    ' loop all columns in the sheet and add them to the datatable
                    For Each cell In ms.Cells(1, 1, 1, ms.Dimension.End.Column)
                        Dim columnName As String = cell.Text.Trim()
                        ' check if the previous head was empty and add it oif it was
                        If (cell.Start.Column <> currentColumn) Then
                            columnNames.Add("Header_" & currentColumn)
                            dt.Columns.Add("Header_" & currentColumn)
                            currentColumn += 1
                        End If
                        ' add the column name to the list to count the duplicates
                        columnNames.Add(columnName)
                        ' count the duplicate column names and make them unique to avoid the exception 
                        Dim occurrences As Integer = columnNames.Where((Function(x As String) x.Equals(columnName))).Count
                        ' A column named 'name' already belongs to this datatable
                        If (occurrences > 1) Then
                            columnName = columnName & "_" & occurrences
                        End If
                        ' add the colum to the datatable
                        dt.Columns.Add(columnName)
                        currentColumn += 1
                    Next
                    '----------------- Row -----------------------------
                    For i As Integer = 2 To ms.Dimension.End.Row
                        Dim row = ms.Cells(i, 1, i, ms.Dimension.End.Column)
                        Dim newRow As System.Data.DataRow = dt.NewRow()
                        For Each cell In row                     ' loop all cells in the row
                            newRow(cell.Start.Column - 1) = cell.Text
                        Next
                        dt.Rows.Add(newRow)
                    Next

                    returnDT = dt

                End Using

                Return returnDT

            End Function

#End Region
        End Class

    End Class

    Public Class FileControl

#Region "<Summary> 파일 있는지 없는지 Check </Summary>"
        Public Function GetFileisExist(ByRef path As String, ByRef KeyPattern As String) As Boolean
            Dim di As IO.DirectoryInfo : Dim files As IO.FileInfo()
            Dim chk As Boolean = False
            Dim tmp As String = Nothing
            Try
                di = New IO.DirectoryInfo(path & "\")
                files = di.GetFiles()
                For Each fi As IO.FileInfo In files
                    tmp = Split(fi.Name, "_")(0)
                    If tmp = KeyPattern Then
                        chk = True
                        Exit For
                    Else
                        chk = False
                    End If
                Next
            Catch ex As Exception
                Debug.Print(Now() & " 결과  " & ex.Message)
            End Try
            Return chk

        End Function
#End Region

#Region "<Summary> 가장 마지막으로 사용한 파일 찾기 </Summary>"
        Public Function GetLastModifyFile(ByRef Path As String, ByRef KeyPattern As String) As String
            ' ex) GetLastModifyFile("C\test\", "찾을파일") 
            Dim di As IO.DirectoryInfo : Dim files As IO.FileInfo()
            Dim chk As Boolean = False : Dim temp As Date
            Dim strFileOut As String = Nothing
            Try
                di = New IO.DirectoryInfo(Path & "\")
                Dim strKey As String = "*" & KeyPattern & "*"
                files = di.GetFiles(strKey, IO.SearchOption.AllDirectories)

                If files.Length < 0 Then
                    chk = False
                Else
                    chk = True
                    For Each a As IO.FileInfo In files
                        Dim lastModif As Date = a.LastWriteTime
                        If lastModif > temp And Not a.ToString().Contains("~$") Then
                            strFileOut = a.FullName
                            Exit For
                        End If
                    Next
                End If

            Catch ex As Exception
                Debug.Print(Now() & " 결과 : " & ex.Message)
            End Try

            Return strFileOut

        End Function
#End Region

#Region "<Summary> 파일 복사하기 </Summary>"
        Public Sub CopyFileTo(ByRef oPath As String, ByRef nPath As String)
            'ex)  CopyFileTo("C:\test\test.exe", "D:\test.test.exe")
            Dim bytesRead As Integer
            Dim buffer(4096) As Byte
            Try  ' oldPath  = 현재 위치 파일 
                Using inFile As New System.IO.FileStream(oPath, IO.FileMode.Open, IO.FileAccess.Read)
                    Using outFile As New System.IO.FileStream(nPath, IO.FileMode.Create, IO.FileAccess.Write)
                        Do
                            bytesRead = inFile.Read(buffer, 0, buffer.Length)
                            If bytesRead > 0 Then
                                outFile.Write(buffer, 0, bytesRead)
                            End If
                        Loop While bytesRead > 0
                    End Using
                End Using
            Catch ex As System.IO.IOException
                Console.WriteLine("Exception Message : {0}", ex.Message)
                Exit Sub
            End Try
        End Sub
#End Region
        Private Function Return_Path_UsingSaveDialog(ByRef Path As String) As String
            Dim return_path As String = Nothing : Dim chk As Boolean = False
            Dim dlg As New Windows.Forms.SaveFileDialog()
            dlg.FileName = IO.Path.GetFileNameWithoutExtension(Path)
            dlg.DefaultExt = IO.Path.GetExtension(Path)
            dlg.Filter = ""
            dlg.Filter = "Excel File|*.xlsm;*.xlsx"
            dlg.Title = "lee.sunbae@lgepartner.com"

            If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
                return_path = dlg.FileName
                chk = True
                'Custom_fileCopy(Path, dlg.FileName)
                'Diagnostics.Process.Start(IO.Path.GetDirectoryName(dlg.FileName))
            Else
                chk = False
            End If

            Return If(chk = True, return_path, Nothing)

        End Function

        Public Sub SaveFileDialog_sun_custom(ByRef Path As String)

            Dim dlg As New Windows.Forms.SaveFileDialog()
            dlg.FileName = IO.Path.GetFileNameWithoutExtension(Path)
            dlg.DefaultExt = IO.Path.GetExtension(Path)
            dlg.Filter = ""
            dlg.Filter = "Excel File|*.xlsm;*.xlsx"
            dlg.Title = "lee.sunbae@lgepartner.com"

            If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Custom_fileCopy(Path, dlg.FileName)
                Diagnostics.Process.Start(IO.Path.GetDirectoryName(dlg.FileName))
            End If
        End Sub

        Private Sub Custom_fileCopy(ByRef oPath As String, ByRef nPath As String)
            '/// Move to File in Server
            Dim line As String = "--------------------------------------------------------------"
            Dim bytesRead As Integer
            Dim buffer(4096) As Byte
            Try  ' oldPath  = 현재 위치 파일 
                Using inFile As New System.IO.FileStream(oPath, IO.FileMode.Open, IO.FileAccess.Read)
                    Using outFile As New System.IO.FileStream(nPath, IO.FileMode.Create, IO.FileAccess.Write)
                        Do
                            bytesRead = inFile.Read(buffer, 0, buffer.Length)
                            If bytesRead > 0 Then
                                outFile.Write(buffer, 0, bytesRead)
                            End If
                        Loop While bytesRead > 0
                    End Using
                End Using
            Catch ex As System.IO.IOException
                Console.WriteLine("Exception Message : {0}", ex.Message) : Console.WriteLine("oldPath : {0}", oPath) : Console.WriteLine("newPath : {0}", nPath)
                Exit Sub
            End Try

        End Sub


        Public Function Custom_folderselect() As String
            Dim PathTo As String = ""
            Dim dialog As CommonOpenFileDialog = New CommonOpenFileDialog
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            dialog.IsFolderPicker = True

            If (dialog.ShowDialog = CommonFileDialogResult.Ok) Then
                PathTo = dialog.FileName
            Else
                PathTo = ""
            End If

            Return PathTo

        End Function



    End Class

    Public Class Clipboards

        Private _dt As DataTable

        Public Sub New()

        End Sub

        Public Function Clipboard_to_DataTable(ByRef pClipboard As String) As DataTable

            ' List<T> 안에 또 List를 생성함. 
            Dim table As List(Of List(Of String)) = New List(Of List(Of String))

            ' Clipboard String을 변수에 담음.
            Dim importText As String = pClipboard

            importText = importText.Replace(vbLf, "")
            ' vbCrLf : 간단히 엔터 값이라고 보면 됨
            ' vbCrLf 를 기준으로 자르기 
            Dim lines() As String = importText.Split(vbCrLf)

            ' Split 된 문자열 배열의 length 만큼 반복
            For i As Integer = 0 To lines.Length - 1

                If (String.IsNullOrEmpty(lines(i))) Then
                    Exit For
                End If

                ' 임시로 String을 담을 List<T> 선언
                Dim cellList As New List(Of String)
                Dim cells() As String = lines(i).Split(vbTab)

                For Each str As String In cells
                    str = str.Replace(Environment.NewLine, String.Empty)
                Next

                ' 임시 List<T>에 값을 담고, 담아진 List를 
                cellList.AddRange(cells)

                ' List 배열에 List를 Add한다.
                table.Add(cellList)

            Next

            ' DataTable을 만든다.
            Dim dt As DataTable = New DataTable
            Dim dCol As DataColumn = New DataColumn
            If (table.Count > 0) Then
                For i As Integer = 0 To table.Item(0).Count - 1
                    With dCol
                        dCol = New DataColumn : .DataType = GetType(String)
                        .ColumnName = "Column_" & i
                        .Caption = "Column_" & i
                        dt.Columns.Add(dCol)
                    End With
                Next
            End If
            ' DataTable에 List 값을 담는다.
            For Each s As List(Of String) In table
                If s.Count > 0 Then
                    dt.Rows.Add(s.ToArray)
                End If
            Next
            ' DataTable을 Return
            Return dt
        End Function

    End Class

    Public Class DbConnection

        Public Class Excel_Class
            Inherits DbConnection

            Private vConn As System.Data.OleDb.OleDbConnection
            Private DS As DataSet
            Private myCmd As System.Data.OleDb.OleDbDataAdapter
            Private _Provider As String = "Provider=Microsoft.Ace.OLEDB.12.0;Data Source="
            Private _Properties As String = ";Extended Properties=""Excel 12.0;HDR=YES;"""
            Public Function Excel_to_datatable(ByRef path As String, ByRef Sheet As String, ByRef sql As String) As DataTable
                Dim connstring As String = String.Format("{0}{1}{2}", _Provider, path, _Properties)

                vConn = New OleDb.OleDbConnection(connstring)

                Try
                    vConn.Open()
                Catch ex As Exception
                    Debugging.Show(ex.Message)
                End Try

                '# Connection 
                Dim dt As DataTable = New DataTable

                '# DB 연결하여 실행
                myCmd = New System.Data.OleDb.OleDbDataAdapter("Select * FROM [" & Sheet & "$A1:P1000]", vConn)

                Try
                    DS = New DataSet
                    myCmd.Fill(DS)      '# DataSet에 엑셀에 있는 내용 모두 담음(조회 된 Query)
                    dt = DS.Tables(0)
                Catch ex As Exception
                    Debugging.Show(ex.Message)
                End Try

                Return If(dt Is Nothing, Nothing, dt)

            End Function

        End Class


        Public Class Access_Class
            Inherits DbConnection
#Region "<Summary> accdb to Datatable </Summary>"
            Public Function Access_to_datatable(ByRef connstring As String, ByRef sql As String) As DataTable
                Dim dt As New System.Data.DataTable
                Using cn As New System.Data.OleDb.OleDbConnection With {.ConnectionString = connstring}
                    Using cmd As New System.Data.OleDb.OleDbCommand With {.Connection = cn}
                        cmd.CommandType = System.Data.CommandType.Text
                        cmd.CommandText = sql
                        cn.Open()
                        Using da As New System.Data.OleDb.OleDbDataAdapter(cmd)
                            Try
                                da.Fill(dt)
                            Catch ex As Exception
                            End Try
                        End Using
                        cn.Close()
                    End Using
                End Using
                Return dt
            End Function
#End Region

        End Class

        Public Class Mysql_Class
            Inherits DbConnection
            Public Sub New()

            End Sub

            Public Function Query_to_Mysql(ByRef sql As String, Optional ByRef connstring As String = "Server=10.169.88.40;Uid=rs_user;Pwd=lge1234;Database=td_defect") As Boolean
                Dim chkErr As Boolean = False
                Using cn As New MySql.Data.MySqlClient.MySqlConnection With {.ConnectionString = connstring}
                    Using cmd As New MySql.Data.MySqlClient.MySqlCommand With {
                    .Connection = cn,
                    .CommandType = System.Data.CommandType.Text,
                    .CommandText = sql}
                        Try
                            cn.Open()
                            cmd.ExecuteNonQuery()
                            chkErr = True
                            Debug.Print("[Query result] > " & "Successfull")
                        Catch ex As Exception
                            Qportals.Debugging.Print(ex.Message)
                            chkErr = False
                        Finally
                            cn.Close()
                        End Try
                    End Using
                    Return If(chkErr = False, False, True)
                End Using
            End Function

#Region "<Summary> mysql to Datatable </Summary>"
            Public Function Mysql_to_datatable(ByRef sql As String, Optional ByRef connstring As String = "Server=10.169.88.40;Uid=rs_user;Pwd=lge1234;Database=td_defect") As DataTable
                Dim dt As New System.Data.DataTable
                'Dim connstring As String = MyBase._connectionString & MyBase._schemaSet
                Using cn As New MySql.Data.MySqlClient.MySqlConnection With {.ConnectionString = connstring}
                    Using cmd As New MySql.Data.MySqlClient.MySqlCommand With {.Connection = cn}
                        cmd.CommandType = System.Data.CommandType.Text
                        cmd.CommandText = sql
                        cn.Open()
                        Using da As New MySql.Data.MySqlClient.MySqlDataAdapter(cmd)
                            Try
                                da.Fill(dt)
                                Debug.Print("[Query result] > " & "Successfull")
                            Catch ex As Exception
                                Debug.Print(ex.Message, "lee.sunbae@lgepartner.com", 0, 64)
                            End Try
                        End Using
                        cn.Close()
                    End Using
                End Using

                Return dt

            End Function
#End Region

#Region "<Summary> Return Query Result of String </Summary>"
            Public Function GetQueryResult(ByRef sql As String, Optional ByRef connstring As String = "Server=10.169.88.40;Uid=rs_user;Pwd=lge1234;Database=td_defect") As String
                Dim result As String = Nothing
                Using cn As New MySql.Data.MySqlClient.MySqlConnection With {.ConnectionString = connstring}
                    Using cmd As New MySql.Data.MySqlClient.MySqlCommand With {.Connection = cn}
                        cmd.CommandType = System.Data.CommandType.Text
                        cmd.CommandText = sql
                        cn.Open()
                        Dim dr As MySql.Data.MySqlClient.MySqlDataReader = cmd.ExecuteReader
                        If dr.HasRows Then
                            dr.Read()
                            result = dr.GetString(0)
                            Debug.Print("[Query result] > " & result)
                        Else
                            result = Nothing
                        End If
                        cn.Close()
                    End Using
                End Using
                Return result
            End Function
#End Region


            Public Function GetContacts(ByRef user As String) As DataTable

                Dim connString As String = "Server=10.169.88.40;Uid=rs_user;Pwd=lge1234;Database=" & "td_defect"
                Dim sql As String

                sql = String.Format("WITH all_tables AS (SELECT `이름`, `직급`, `업체`, `부서` FROM td_defect.`contacts_c`
                UNION ALL SELECT `이름`, `직급`, `업체`, `부서` FROM td_defect.`contacts_i`
                UNION ALL SELECT `이름`, `직급`, `업체`, `부서` FROM td_defect.`contacts_m`
                )
                SELECT * FROM all_tables
                WHERE `이름` = '{0}'", user)


                Dim dt As DataTable = Mysql_to_datatable(sql, connString)

                Return If(dt, Nothing)
            End Function


        End Class

    End Class

    Public Class Controls

        Public Class InputBoxs

            Public Function ShowInputBox(ByRef msg() As String, r As Rectangle) As String
                Dim inputString As String = Nothing

                Dim x = r.Left + (r.Width) / 5
                Dim y = r.Top + (r.Height) / 5

                inputString = InputBox(msg(0), msg(1), msg(2), x, y)

                Return If(inputString Is Nothing Or inputString = "", Nothing, inputString)


            End Function


        End Class

        Public Class ListBoxClass
            Public Property _ListBox As ListBox

            Public Sub AddListBox(ByRef _dt As DataTable, ByRef _columnName As String)
                Dim temp_dt As DataTable = _dt

                If Not (temp_dt Is Nothing) And Not (_dt.Rows.Count = 0) Then
                    temp_dt = temp_dt.DefaultView.ToTable(True, _columnName)
                    For i As Integer = 0 To temp_dt.Rows.Count - 1
                        _ListBox.Items.Add(temp_dt.Rows(i)(0).ToString())
                    Next
                End If

            End Sub

            Public Sub ClickItems(ByRef lstBoxs() As ListBox, ByRef _dt As DataTable)

                Select Case lstBoxs.Length
                    Case 2
                        _ListBox = lstBoxs(1)
                        _ListBox.Items.Clear()
                        For Each dr As DataRow In _dt.Rows
                            If lstBoxs(0).SelectedItem.ToString = dr.Item(1).ToString Then
                                If Not (_ListBox.Items.Contains(dr.Item(2).ToString)) Then
                                    _ListBox.Items.Add(dr.Item(2).ToString)
                                End If
                            End If
                        Next
                    Case 3
                        _ListBox = lstBoxs(2)
                        _ListBox.Items.Clear()
                        For Each dr As DataRow In _dt.Rows
                            If (lstBoxs(0).SelectedItem.ToString = dr.Item(1).ToString) And
                                (lstBoxs(1).SelectedItem.ToString = dr.Item(2).ToString) Then
                                If Not (_ListBox.Items.Contains(dr.Item(3).ToString)) Then
                                    _ListBox.Items.Add(dr.Item(3).ToString)
                                End If
                            End If
                        Next
                    Case 4
                        _ListBox = lstBoxs(3)
                        _ListBox.Items.Clear()
                        For Each dr As DataRow In _dt.Rows
                            If (lstBoxs(0).SelectedItem.ToString = dr.Item(1).ToString) And
                                (lstBoxs(1).SelectedItem.ToString = dr.Item(2).ToString) And
                                (lstBoxs(2).SelectedItem.ToString = dr.Item(3).ToString) Then
                                If Not (_ListBox.Items.Contains(dr.Item(4).ToString)) Then
                                    _ListBox.Items.Add(dr.Item(4).ToString)
                                End If
                            End If
                        Next
                End Select


            End Sub


            Public Sub ClickNextItem_One_Depth(ByRef lstbox As ListBox, ByRef lstbox2 As ListBox, ByRef _dt As DataTable, ByRef col As Integer)

                ' Linq Query
                lstbox2.Items.Clear()
                For Each dr As DataRow In _dt.Rows
                    If lstbox.SelectedItem.ToString = dr.Item(col).ToString Then
                        If Not (lstbox2.Items.Contains(dr.Item(col + 1))) Then
                            lstbox2.Items.Add(dr.Item(col + 1))
                        End If
                    End If
                Next

            End Sub



        End Class

        Public Class TreeListViewMaker
            Private trv As TreeListView = New TreeListView
            Public Property _trv As TreeListView
                Get
                    Return trv
                End Get
                Set(value As TreeListView)
                    trv = value
                End Set
            End Property
            Public Sub Main()
                trv.MultiSelect = False
                trv.FullRowSelect = True
                trv.CheckBoxes = True
                With trv.Columns
                    '.Add("TC_Info", 300, HorizontalAlignment.Center).Font = New Font("맑은 고딕", 8, FontStyle.Italic Or FontStyle.Bold)
                    '.Add("표준M/D", 100, HorizontalAlignment.Center).Font = New Font("맑은 고딕", 8, FontStyle.Italic Or FontStyle.Bold)
                    .Item(0).Font = New Font("맑은 고딕", 7, FontStyle.Italic Or FontStyle.Bold)
                    .Item(1).Font = New Font("맑은 고딕", 7, FontStyle.Italic Or FontStyle.Bold)
                    .Item(1).ForeColor = Color.Blue
                    .Item(2).Font = New Font("맑은 고딕", 7, FontStyle.Italic Or FontStyle.Bold)
                End With

                trv.RowSelectColor = Color.FromArgb(190, 216, 242)
                trv.DisabledColor = Color.FromArgb(190, 216, 242)

            End Sub

            Private _columns() As Integer
            Private _lastIndexOfColumns As Integer
            Public Overloads Sub BuildTree(ByRef _dt As DataTable, columns() As Integer, Optional expandAll As Boolean = False)
                _columns = columns
                _lastIndexOfColumns = columns.Length - 1 ' 실제 마지막 columnindex
                _trv.BeginUpdate()
                _trv.Nodes.Clear()
                '검색하면서 노드 추가
                For Each row As DataRow In _dt.Rows
                    Dim node As TreeListNode
                    Dim columnIndex As Integer = 0
                    node = SearchNode(Nothing, row, columnIndex)
                    If (node Is Nothing) Or (columnIndex < _columns.Length) Then
                        AddNode(node, row, columnIndex)
                    ElseIf (columnIndex = _lastIndexOfColumns) Then

                    End If
                Next
                If expandAll Then
                    _trv.ExpandAll()
                End If
                _trv.EndUpdate()
            End Sub

            Private Function SearchNode(parent As TreeListNode, row As DataRow, ByRef columnIndex As Integer) As TreeListNode
                'Dim nodes As TreeNodeCollection
                Dim nodes As Collections.TreeListNodeCollection
                Dim node As TreeListNode = Nothing
                ' 만약 상위 노드가 없다면 treeview의 노드, 아니면 parent의 노드로
                nodes = If(parent Is Nothing, _trv.Nodes, parent.Nodes)
                For Each item As TreeListNode In nodes
                    If String.CompareOrdinal(item.Text, row(_columns(columnIndex)).ToString()) = 0 Then
                        columnIndex = columnIndex + 1
                        If columnIndex >= _columns.Length Then
                            Return item
                        Else
                            Return SearchNode(item, row, columnIndex) ' 재귀 함수
                        End If
                    Else
                        node = item.ParentNode
                    End If
                Next
                Return If(columnIndex > 0, node, Nothing)
            End Function
            '노드 추가
            '   parent          노드를 추가할 부모 노드
            '   row             참조할 테이블의 행
            '   columnIndex     참조할 테이블의 열 번호 배열(_columns)에서 현재 사용할 인덱스
            Private Sub AddNode(ByRef parent As TreeListNode, row As DataRow, ByRef columnIndex As Integer)

                If parent Is Nothing Then
                    _trv.Nodes.Add(row(_columns(0)).ToString()).Font = New Font("맑은 고딕", 8)
                    parent = _trv.Nodes(_trv.GetNodeCount(False) - 1) ' GetNodeCount : 노드개수 trv의 노드 개수
                    columnIndex = columnIndex + 1
                End If

                For i As Integer = columnIndex To _lastIndexOfColumns   '** 마지막 컬럼은 List로 들어가도록
                    parent.Nodes.Add(row(_columns(i)).ToString()).Font = New Font("맑은 고딕", 8)
                    If (i = _lastIndexOfColumns) Then
                        parent.SubItems.Add(row(_columns(i)).ToString())
                    Else
                        parent.SubItems.Add(row(_columns(i)).ToString())
                        parent = parent.Nodes(parent.GetNodeCount(False) - 1)
                    End If
                Next
            End Sub

            Public Sub CheckBoxAll(node As TreeListNode, checkNode As Boolean)
                For Each child As TreeListNode In node.Nodes
                    child.Checked = checkNode
                    If (child.Nodes.Count > 0) Then
                        CheckBoxAll(child, checkNode)
                    End If
                Next
            End Sub

            Public Sub TreeListViewToListView(nodes As TreeListNodeCollection, ListView As ListView)
                Dim temp As String : Dim temps() As String
                Dim lvi As ListViewItem = Nothing
                For Each node As TreeListNode In nodes
                    If node.Checked Then
                        If Not (node.ParentNode Is Nothing) Then
                            temp = node.FullPath
                            If temp.Split("\").Count = 3 Then
                                temps = temp.Split("\")
                                lvi = New ListViewItem(temps(1))
                                lvi.SubItems.Add(temps(2)) : lvi.SubItems.Add(temps(0))
                                ListView.Items.Add(lvi)
                            End If
                        End If
                    End If
                    TreeListViewToListView(node.Nodes, ListView)
                Next
            End Sub

            Property drows As List(Of List(Of String)) = New List(Of List(Of String))
            Property dgv As DataGridView
            Property dt As DataTable
            Private Sub TrvToDgva(nodes As TreeListNodeCollection, ByRef dt As DataTable)
                Dim temps() As String
                Dim temp As String = Nothing

                temps = New String() {"Project", "GroupName", "Model", "Step", "Start_Date", "End Date", temp(0), temp(1), "Tester", "Part", "업체명"}

            End Sub
            Private Function TrvToDgv(nodes As TreeListNodeCollection, cols() As String) As List(Of List(Of String))
                Dim temps() As String
                Dim temp As String = Nothing
                Dim drow As List(Of String) = New List(Of String)

                For Each node As TreeListNode In nodes
                    If (node.Checked) Then
                        If Not (node.ParentNode Is Nothing) Then
                            temp = node.FullPath
                            If temp.Split("\").Count = 2 Then   ' [최하위 선택인지]
                                For i As Integer = 0 To cols.Length - 1
                                    drow.Add(cols(i))
                                Next
                                drows.Add(drow)
                                drow = New List(Of String)
                            End If
                        End If
                    End If
                    TrvToDgv(node.Nodes, cols)
                Next

                Return drows

            End Function


            Public Sub TreeListViewToDataGridView(nodes As TreeListNodeCollection, cols() As String)

                Dim row As DataRow
                For Each li As List(Of String) In drows
                    row = dt.Rows.Add()
                    row.ItemArray = li.ToArray()
                    dt.AcceptChanges()
                Next
                'Qportals.Debugging.Show("test")

            End Sub

            Property _startDate As DateTimePicker
            Property _endDate As DateTimePicker
            Public Sub Make_Node_Project(ByRef _dt As DataTable, ByVal Trv As TreeListView, Optional Collab As Boolean = False)

                Trv.Nodes.Clear()
                Dim node_1 As TreeListNode = Nothing
                Dim node_2 As TreeListNode = Nothing
                Dim node_3 As TreeListNode = Nothing
                Dim node_4 As TreeListNode = Nothing

                Dim nodeName_1 As String = ""
                Dim nodeName_2 As String = ""
                Dim nodeName_3 As String = ""
                Dim nodeName_4 As String = ""

                Dim ProjectTableName As String = ""

                Dim change_depth1 As Boolean = False

                Trv.BeginUpdate()

                For Each row As DataRow In _dt.Rows

                    ' 1 Depth
                    If Not IsDBNull(row.Item(0)) Then
                        If row.Item(0).ToString <> nodeName_1 Then
                            node_1 = New TreeListNode(row.Item(0))
                            Trv.Nodes.Add(node_1)
                            'Trv.Nodes.Item(Trv.Nodes.Count - 1).SubItems.Add(row.Item(3).ToString)
                            'Trv.Nodes.Item(Trv.Nodes.Count - 1).SubItems.Add(row.Item(3).ToString)
                            'Trv.Nodes.Item(Trv.Nodes.Count - 1).SubItems.Add(row.Item(10).ToString)
                            'Trv.Nodes.Item(Trv.Nodes.Count - 1).SubItems.Add(row.Item(11).ToString)
                            'Trv.Nodes.Item(Trv.Nodes.Count - 1).SubItems.Add(row.Item(12).ToString)
                            nodeName_1 = row.Item(0).ToString
                            nodeName_2 = Nothing
                            nodeName_3 = Nothing
                            nodeName_4 = Nothing

                        End If
                    End If

                    If row.Item(1).ToString = "Device" Then
                        Qportals.Debugging.Print(row.Item(1).ToString)
                    End If

                    ' 2 Depth
                    If Not IsDBNull(row.Item(1)) Then
                        If row.Item(1).ToString <> nodeName_2 Then
                            node_2 = New TreeListNode(row.Item(1))
                            node_1.Nodes.Add(node_2)
                            node_1.Nodes.Item(node_1.Nodes.Count - 1).Font = New Font("맑은 고딕", 7.25)
                            'node_1.Nodes.Item(node_1.Nodes.Count - 1).SubItems.Add(row.Item(2).ToString)

                            'node_1.Nodes.Item(node_1.Nodes.Count - 1).SubItems.Add(row.Item(3).ToString)
                            'node_1.Nodes.Item(node_1.Nodes.Count - 1).SubItems.Add(row.Item(10).ToString)
                            'node_1.Nodes.Item(node_1.Nodes.Count - 1).SubItems.Add(row.Item(11).ToString)
                            'node_1.Nodes.Item(node_1.Nodes.Count - 1).SubItems.Add(row.Item(12).ToString)
                            nodeName_2 = row.Item(1).ToString
                            nodeName_3 = Nothing
                            nodeName_4 = Nothing
                        End If
                    End If

                    ' 3 Depth
                    node_3 = New TreeListNode(row.Item(2))
                    node_2.Nodes.Add(node_3)
                    node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(3).ToString)  ' 이름 ex) 이순배
                    If (row.ItemArray.Length = 5) Then
                        node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(4).ToString) ' 역량 ex) 4.4
                    End If
                    node_2.Nodes.Item(node_2.Nodes.Count - 1).Font = New Font("맑은 고딕", 7.25)
                    'node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(5).ToString) ' 비고 

                    node_3.ImageIndex = -1
                    node_3.SelectedImageIndex = -1
                    'node_3.Nodes.Item(node_3.Nodes.Count - 1).Font = New Font("맑은 고딕", 7.25)
                    '        node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(3).ToString)
                    '        'node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(10).ToString)
                    '        'node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(11).ToString)
                    '        'node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(12).ToString)
                    nodeName_3 = row.Item(2).ToString
                    nodeName_4 = Nothing




                    ' 3 Depth
                    'If Not IsDBNull(row.Item(3)) Then
                    '    If row.Item(3).ToString <> nodeName_3 Then
                    '        node_3 = New TreeListNode(row.Item(2))
                    '        node_2.Nodes.Add(node_3)
                    '        node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(3).ToString)  ' 이름 ex) 이순배
                    '        node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(4).ToString) ' 역량 ex) 4.4
                    '        'node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(5).ToString) ' 비고 

                    '        node_3.ImageIndex = -1
                    '        node_3.SelectedImageIndex = -1
                    '        'node_3.Nodes.Item(node_3.Nodes.Count - 1).Font = New Font("맑은 고딕", 7.25)
                    '        '        node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(3).ToString)
                    '        '        'node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(10).ToString)
                    '        '        'node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(11).ToString)
                    '        '        'node_2.Nodes.Item(node_2.Nodes.Count - 1).SubItems.Add(row.Item(12).ToString)
                    '        nodeName_3 = row.Item(2).ToString
                    '        nodeName_4 = Nothing
                    '    End If
                    'End If

                    'If Not IsDBNull(row.Item(4)) Then
                    '    If row.Item(4).ToString <> nodeName_4 Then
                    '        node_4 = New TreeListNode(row.Item(4))
                    '        node_3.Nodes.Add(node_4)
                    '        node_3.Nodes.Item(node_3.Nodes.Count - 1).Font = New Font("맑은 고딕", 8)
                    '        node_3.Nodes.Item(node_3.Nodes.Count - 1).SubItems.Add(row.Item(2).ToString)
                    '        node_3.Nodes.Item(node_3.Nodes.Count - 1).SubItems.Add(row.Item(3).ToString)
                    '        'node_3.Nodes.Item(node_3.Nodes.Count - 1).SubItems.Add(row.Item(10).ToString)
                    '        'node_3.Nodes.Item(node_3.Nodes.Count - 1).SubItems.Add(row.Item(11).ToString)
                    '        'node_3.Nodes.Item(node_3.Nodes.Count - 1).SubItems.Add(row.Item(12).ToString)
                    '        '_startDate = Convert.ToDateTime(row.Item(6).ToString())
                    '        '_endDate = Convert.ToDateTime(row.Item(7).ToString())
                    '        'If Date.Now().ToString("yyyy-MM-dd HH:mm:ss") <= EndDate And Date.Now().ToString("yyyy-MM-dd HH:mm:ss") >= StartDate Then
                    '        '    node_3.Nodes.Item(node_3.Nodes.Count - 1).BackColor = Color.FromArgb(235, 255, 234)
                    '        'Else
                    '        '    node_3.Nodes.Item(node_3.Nodes.Count - 1).BackColor = Color.FromArgb(255, 255, 255)
                    '        'End If
                    '        'nodeName_4 = row.Item(4).ToString
                    '    End If
                    'End If
                Next row

                Trv.EndUpdate()
                If Collab = True Then
                    Trv.ExpandAll()
                End If

            End Sub



        End Class

        Public Class LviClass

            Public Sub CtrlA(ByRef ListView As ListView, e As KeyEventArgs)
                If (e.Control) Then
                    If (e.KeyCode = Keys.A) Then
                        If Not (ListView.Items.Count = 0) Then
                            For i As Integer = ListView.Items.Count - 1 To 0 Step -1
                                ListView.Items(i).Selected = True
                            Next
                        End If
                    End If
                End If

            End Sub

            'Public Sub UnSelectItemcolor(ByRef ListView As ListView, e As EventArgs)
            '    ListView.Items.Cast(Of ListViewItem)().ToList().ForEach(Sub(item)
            '                                                                item.BackColor = SystemColors.Info
            '                                                                item.ForeColor = SystemColors.WindowText
            '                                                            End Sub)
            '    ListView.SelectedItems.Cast(Of ListViewItem)().ToList().ForEach(Sub(item)
            '                                                                        item.BackColor = SystemColors.Highlight
            '                                                                        item.ForeColor = SystemColors.HighlightText
            '                                                                    End Sub)
            'End Sub

            'Public Sub CustomColumnHeader(ByRef ListView As ListView, e As DrawListViewColumnHeaderEventArgs)
            '    'ListView.OwnerDraw = True

            '    'e.DrawBackground()
            '    'Dim txtpos As Point = New Point(e.Bounds.X + 3, e.Bounds.Y + 2)
            '    'If e.Header.Index = 0 Then
            '    '    e.Graphics.DrawString(e.Header.Text, e.Font, Brushes.Red, txtpos)
            '    'ElseIf e.Header.Index = 1 Then
            '    '    e.Graphics.DrawString(e.Header.Text, e.Font, Brushes.Blue, txtpos)
            '    'End If

            '    'e.Graphics.FillRectangle(Brushes.Pink, e.Bounds)
            '    'e.DrawText()
            'End Sub


        End Class

        Public Class TxtBoxStyle

            Public Class RemoveExampleText

                Private __textBox As TextBox
                Property _textBox As TextBox
                    Get
                        Return __textBox
                    End Get
                    Set(value As TextBox)
                        __textBox = value
                    End Set
                End Property

                Public Sub RemoveTextWhenEx(ByRef textBox As TextBox, Optional ByRef removeText As String = "ex)")
                    If (Left(textBox.Text.ToString, 3).ToLower.Contains(removeText.ToLower)) Then
                        textBox.Text = ""
                        textBox.ForeColor = Color.Black
                    End If

                End Sub


            End Class



        End Class

        Public Class dgvStyle
            Public Sub New()

            End Sub

            Public Sub Grid_RowPostPaint(ByRef _datagridview1 As DataGridView, ByRef e As DataGridViewRowPostPaintEventArgs)

                If (_datagridview1.RowCount > 1) And (_datagridview1.CurrentCell IsNot Nothing) Then
                    If e.RowIndex = _datagridview1.CurrentCell.RowIndex Then
                        ' Calculate the bounds of the row 
                        Dim rowHeaderWidth As Integer =
                            If(_datagridview1.RowHeadersVisible,
                               _datagridview1.RowHeadersWidth, 0)
                        Dim rowBounds As New Rectangle(
                        rowHeaderWidth,
                        e.RowBounds.Top,
                        _datagridview1.Columns.GetColumnsWidth(
                                DataGridViewElementStates.Visible) -
                                _datagridview1.HorizontalScrollingOffset + 1,
                        e.RowBounds.Height)

                        ' Paint the border 
                        ControlPaint.DrawBorder(e.Graphics, rowBounds,
                                     Color.Green, ButtonBorderStyle.Solid)

                        ' Paint the background color 
                        _datagridview1.Rows(e.RowIndex).DefaultCellStyle.BackColor =
                                                    Color.LightGray
                    Else
                        ' 선택 해제 했을 때
                        Dim rowHeaderWidth As Integer = If(_datagridview1.RowHeadersVisible, _datagridview1.RowHeadersWidth, 0)
                        Dim rowBounds As New Rectangle(rowHeaderWidth, e.RowBounds.Top, _datagridview1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) -
                                _datagridview1.HorizontalScrollingOffset + 1, e.RowBounds.Height)

                        ' Paint the border 
                        ControlPaint.DrawBorder(e.Graphics, rowBounds, Color.DarkGray, ButtonBorderStyle.Solid)

                        ' Paint the background color 
                        _datagridview1.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.White
                    End If

                End If



            End Sub



        End Class


        Public Class ListViewMaker
            Public Sub New()

            End Sub
            Private listview As ListView
            Public Property _listview As ListView
                Get
                    Return listview
                End Get
                Set(value As ListView)
                    listview = value
                End Set
            End Property
            Private dttable As DataTable
            Public Property _dtable As DataTable
                Get
                    Return dttable
                End Get
                Set(value As DataTable)
                    dttable = value
                End Set
            End Property
            Private _columns() As Integer

            Public Sub BuildList(ByRef listview As ListView, ByRef dt As DataTable, ByRef columns() As Integer)
                _listview = listview
                _dtable = dt
                _columns = columns

                Dim itemcol As List(Of String) = New List(Of String)  '** ListView의 열 데이터를 넣어주기 위해 만들었습니다.

                ' ListView의 컬럼을 생성하기 위해 만들었습니다.
                For Each col As DataColumn In _dtable.Columns
                    _listview.Columns.Add(col.ColumnName.ToString)
                Next

                For i As Integer = 0 To _dtable.Rows.Count - 1

                    For j As Integer = 0 To _columns.Length - 1
                        itemcol.Add(dt.Rows(i)(j).ToString())
                    Next
                    Dim lvi As New ListViewItem(itemcol.ToArray)
                    _listview.Items.Add(lvi)
                    itemcol = New List(Of String)

                Next

            End Sub

        End Class

        Public Class TreeViewMaker

            Private _columns() As Integer  '** 참조할 테이블의 열 번호 배열 (멤버 함수로 전달할 인자를 줄이기 위해 편의상 만들었습니다).
            Private _lastIndexOfColumns As Integer  '** _columns 멤버의 마지막 인덱스 (멤버 함수로 전달할 인자를 줄이기 위해 편의상 만들었습니다.)
            Private _tview As TreeView
            Private dttable As DataTable
            Public Property _dtable As DataTable
                Get
                    Return dttable
                End Get
                Set(value As DataTable)
                    dttable = value
                End Set
            End Property
            Public Property TView As TreeView
                Get
                    Return _tview
                End Get
                Set(value As TreeView)
                    _tview = value
                End Set
            End Property

            '트리 만들기
            '   depth           트리의 최대 깊이
            '   columns         참조할 테이블의 열 번호 배열
            '   expandAll       노드 확장 여부. 기본 값은 확장하지 않음
            Public Overloads Sub BuildTree(ByRef dt As DataTable, ByRef trv As TreeView, columns() As Integer, Optional expandAll As Boolean = False)
                TView = trv
                _dtable = dt
                _columns = columns
                _lastIndexOfColumns = columns.Length - 1 ' 실제 마지막 columnindex
                _tview.BeginUpdate()
                _tview.Nodes.Clear()

                '검색하면서 노드 추가
                For Each row As DataRow In _dtable.Rows
                    Dim node As TreeNode
                    Dim columnIndex As Integer = 0

                    node = SearchNode(Nothing, row, columnIndex)

                    If (node Is Nothing) Or (columnIndex < _columns.Length) Then
                        AddNode(node, row, columnIndex)
                    End If
                Next

                If expandAll Then
                    TView.ExpandAll()
                End If

                _tview.EndUpdate()
            End Sub

            '노드 검색
            '   parent          검색에 사용할 노드 집합의 부모 노드
            '   row             참조할 테이블의 행
            '   columnIndex     참조할 테이블의 열 번호 배열(_columns)에서 현재 사용할 인덱스
            Private Function SearchNode(parent As TreeNode, row As DataRow, ByRef columnIndex As Integer) As TreeNode
                Dim nodes As TreeNodeCollection
                Dim node As TreeNode = Nothing

                ' 만약 상위 노드가 없다면 treeview의 노드, 아니면 parent의 노드로
                nodes = If(parent Is Nothing, _tview.Nodes, parent.Nodes)

                For Each item As TreeNode In nodes
                    If String.CompareOrdinal(item.Text, row(_columns(columnIndex)).ToString()) = 0 Then
                        columnIndex = columnIndex + 1

                        If columnIndex >= _columns.Length Then
                            Return item
                        Else
                            Return SearchNode(item, row, columnIndex) ' 재귀 함수
                        End If
                    Else
                        node = item.Parent
                    End If
                Next

                Return If(columnIndex > 0, node, Nothing)
            End Function

            '노드 추가
            '   parent          노드를 추가할 부모 노드
            '   row             참조할 테이블의 행
            '   columnIndex     참조할 테이블의 열 번호 배열(_columns)에서 현재 사용할 인덱스
            Private Sub AddNode(ByRef parent As TreeNode, row As DataRow, ByRef columnIndex As Integer)
                If parent Is Nothing Then
                    _tview.Nodes.Add(row(_columns(0)).ToString())
                    parent = _tview.Nodes(_tview.GetNodeCount(False) - 1)
                    columnIndex = columnIndex + 1
                End If

                For i As Integer = columnIndex To _lastIndexOfColumns
                    parent.Nodes.Add(row(_columns(i)).ToString())
                    parent = parent.Nodes(parent.GetNodeCount(False) - 1)
                Next
            End Sub

            Public Sub CheckBoxAll(node As TreeNode, checkNode As Boolean)
                For Each child As TreeNode In node.Nodes
                    child.Checked = checkNode
                    If (child.Nodes.Count > 0) Then
                        CheckBoxAll(child, checkNode)
                    End If
                Next
            End Sub

            Public Sub TreeViewToListView(nodes As TreeNodeCollection, ListView As ListView)
                Dim temp As String : Dim temps() As String
                Dim lvi As ListViewItem = Nothing
                For Each node As TreeNode In nodes
                    If node.Checked Then
                        If Not (node.Parent Is Nothing) Then
                            temp = node.FullPath
                            If temp.Split("\").Count = 3 Then
                                temps = temp.Split("\")
                                lvi = New ListViewItem(temps(1))
                                lvi.SubItems.Add(temps(2)) : lvi.SubItems.Add(temps(0))
                                ListView.Items.Add(lvi)
                            End If
                        End If
                    End If
                    TreeViewToListView(node.Nodes, ListView)
                Next
            End Sub

        End Class

        Public Class SlidePanel
            Private __slidePanel As Panel
            Private __slideButton As Panel
            Property _slidePanel As Panel   ' Panel을 담을 Property 생성
                Get
                    Return __slidePanel
                End Get
                Set(value As Panel)
                    __slidePanel = value
                End Set
            End Property
            Property _slideButton As Panel ' Button을 담을 Property 생성
                ' Class 내부에서 Button을 쓰려고..
                Get
                    Return __slideButton
                End Get
                Set(value As Panel)
                    __slideButton = value
                End Set
            End Property
            Public Sub Panel_OpenSlide(ByRef StartNum As Integer, ByRef EndNum As Integer)
                ' StartNum 부터 EndNum까지 증가시켜서 패널을 늘린다.
                For i As Integer = StartNum To EndNum
                    If (_slidePanel.InvokeRequired) Then ' CrossThreadException을 방지하기 위해 Invoke 처리 함.
                        _slidePanel.Invoke(Sub() _slidePanel.Height = i) ' 패널의 Height 를 늘린다.
                        _slideButton.Invoke(Sub() _slideButton.Top = _slidePanel.Height) ' 패널이 늘려지면 버튼도 같이 늘려야 하기 때문
                    Else
                        _slidePanel.Height = i
                        _slideButton.Top = _slidePanel.Height
                        'Threading.Thread.Sleep(1)
                    End If
                Next
            End Sub
            Public Sub Panel_CloseSlide(ByRef StartNum As Integer, ByRef EndNum As Integer)
                ' 거꾸로 다시 줄어들게 함.
                For i As Integer = StartNum To EndNum Step -1
                    If _slidePanel.InvokeRequired Then
                        _slidePanel.Invoke(Sub() _slidePanel.Height = i)
                        _slideButton.Invoke(Sub() _slideButton.Top = _slidePanel.Height)
                    Else
                        _slidePanel.Height = i
                        _slideButton.Top = _slidePanel.Height
                        'Threading.Thread.Sleep(1)
                    End If
                Next
            End Sub
        End Class


    End Class

    Public Class ComputerInfo

#Region " get User Name "
        Public Function getUserName() As String
            ' ex) Dim name as String = getUserName()
            Dim strCom As String = "."
            Dim strName As String = Nothing
            Dim obj = GetObject("winmgmts:{impersonationLevel=impersonate}!\\" & strCom & "\root\cimv2").ExecQuery("Select * FROM Win32_OperatingSystem")
            For Each Obj2 In obj
                strName = Obj2.Description
            Next
            strName = Strings.Split(strName, "/")(0)
            Return strName
        End Function
#End Region

#Region " get Company "
        Public Function GetContact(ByRef user As String, Optional ByRef table As String = "contact") As DataTable
            Dim dbc As Qportals.DbConnection.Mysql_Class = New DbConnection.Mysql_Class

            Dim sql As String

            sql = String.Format("select * from rs2.`{0}` where `협력사` like '%{1}%'", table, user)

            Dim dt As DataTable = dbc.Mysql_to_datatable(sql)

            Return If(dt, Nothing)
        End Function



#End Region
#Region "Check Admin"

        Private Function LeaderCheck() As Boolean

            Dim info As Qportals.ComputerInfo = New Qportals.ComputerInfo
            Dim _user As String = info.getUserName
            Dim UserCompany As String = Nothing, UserPart As String = Nothing

            '* 2019-09-03 검증원 정보 수정
            '* 이름을 가지고 회사명을 가져옵니다.
            Dim dt_comp As System.Data.DataTable = info.GetContact(_user)
            'Dim dt_comp As System.Data.DataTable = info.GetContact("이호섭")
            Try
                UserCompany = dt_comp.Rows(0).Item(2).ToString   ' 업체
            Catch ex As Exception
                Qportals.Debugging.Show("등록되지 않은 인원 입니다. " & vbCrLf & "관리자에게 문의하여 인원 등록 바랍니다.")
            End Try

            If (UserCompany.Contains(">")) Then
                UserCompany = UserCompany.Split(">")(1)
                UserCompany = Trim(UserCompany)
            End If

            Dim sql As String = String.Format("select `관리자` from rs2.`contact` where `협력사` LIKE '{0}%' And `조직` like '%{1}%'", _user, UserCompany)

            Dim dbc As Qportals.DbConnection.Mysql_Class = New Qportals.DbConnection.Mysql_Class
            Dim strResult As String = dbc.GetQueryResult(sql)


            Return If(strResult = "1", True, False)

        End Function


#End Region



    End Class

    Public Class Debugging

        Public Sub New()

        End Sub
        Public Class Debugging

        End Class
        Private s As String
        Property _input_description As String
            Get
                Return Me.s
            End Get
            Set(value As String)
                Me.s = value
            End Set
        End Property
        Public Overloads Shared Sub Print(ByRef s As String)
            Dim des As String = s
            Dim d As String = DateTime.Now.ToString("yyyyMMddHHmmss")
            des = String.Format("{0} : {1}", d, s)
            Diagnostics.Debug.Print(des)
        End Sub

        Public Overloads Shared Function Log(ByRef s As String) As String
            Dim des As String = s
            Dim d As String = DateTime.Now.ToString("yyyyMMddHHmmss")
            des = String.Format("{0} : {1} {2}", d, s, vbCrLf)
            Return des
        End Function

        Public Overloads Shared Function Show(ByRef s As String, Optional ByRef title As String = "lee.sunbae@lgepartner.com", Optional ByRef buttonOption1 As Integer = 0, Optional ByRef buttonIcon As Integer = 64) As DialogResult
            Using form = New Windows.Forms.Form() With {.TopMost = True} ' 최상위로 포커스
                Windows.Forms.MessageBox.Show(form, s, title, buttonOption1, buttonIcon)
                form.Close()
                form.Dispose()
            End Using
        End Function

    End Class

    Public Class DataGridViewDoubleBuffer
        Inherits DataGridView
        Private _dgv As DataGridView
        Public Sub New(ByRef dgv As DataGridView)
            _dgv = dgv
        End Sub
        Public Sub EnableDoubleBuffered()
            Dim dgvType As Type = _dgv.[GetType]()
            Dim pi As Reflection.PropertyInfo = dgvType.GetProperty("DoubleBuffered",
                                                         Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
            pi.SetValue(_dgv, True, Nothing)
        End Sub
        ' 출처 : https://www.codeproject.com/Tips/1111155/Enable-DataGridView-DoubleBuffered-Property
    End Class



End Namespace


