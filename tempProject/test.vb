Imports System.Collections.Generic
Imports System.Data
    Imports System.Drawing
    Imports System.Linq
    Imports System.Threading
    Imports System.Windows
    Imports System.Windows.Forms
    Imports WinControls.ListView
    Imports WinControls.ListView.Collections
    Imports WinControls.ListView.EventArgClasses

Public Class test

    Public lc As Qportals.Controls.TreeViewMaker = New Qportals.Controls.TreeViewMaker
    Public lviMaker As Qportals.Controls.LviClass = New Qportals.Controls.LviClass

    Private __dtGrid As DataGridView
    Property _dtGrid As DataGridView
        Get
            Return __dtGrid
        End Get
        Set(value As DataGridView)
            __dtGrid = value
        End Set
    End Property
    Private __dtable As DataTable
    Property _dtable As DataTable
        Get
            Return __dtable
        End Get
        Set(value As DataTable)
            __dtable = value
        End Set
    End Property
    Public Sub New()

        InitializeComponent() : m = Me

        _dtGrid = DataGridView1
        '-----------About ListView---------------
        'ListView1.Columns.Add("T/C Type")
        'ListView1.Columns.Add("Test Item")
        'ListView1.Columns.Add("Step")
        'ListView1.Columns(1).Width = 250
        'ListView1.View = View.Details
        'ListView1.FullRowSelect = True
        'ListView1.OwnerDraw = True
        ' ListView1.HeaderStyle = ColumnHeaderStyle.Nonclickable

        '----------DataGridView----------------
        _dtGrid.DefaultCellStyle.Font = New Font("맑은 고딕", 7)
        _dtGrid.ColumnHeadersDefaultCellStyle.Font = New Font("맑은 고딕", 7)


        '------About TreeView-------------------
        'TreeView1.CheckBoxes = True
        'treeView = TreeView1


        '------Side Panel(올리고 내리고)---------
        Slide_panel.Height = 123
        Slide_btnPanel.Top = Slide_panel.Height

        '---------------------------------------
        Dim strcolumns() As String = New String() _
            {"Project", "GroupName", "Model", "Step", "Start_Date", "End Date", "TCType", "TCName", "Tester", "Part", "업체명"}

        '** DataGridView(하단)에 자료를 넣기 위해 DataTable을 만들었습니다.
        '** 이하 dgv 
        _dtable = New DataTable()

        '** Column을 만듭니다.
        For Each c As String In strcolumns
            _dtable.Columns.Add(New DataColumn(c))
        Next
        '** dgv에 자료를 넣어 줍니다.
        If Not (_dtable Is Nothing) Then
            _dtGrid.DataSource = _dtable
        End If
        trvListMaker._trv = TreeListView1
        trvListMaker.Main()


        'AddHandler ListView1.DrawColumnHeader, AddressOf ListViewCustomHeaderColor

        Slide_panel.BringToFront()
        slideButton.BringToFront()
        TreeListView1.AllowDrop = True


        Dim cm As ContextMenuStrip = New ContextMenuStrip
        cm.Items.Add("수정(Edit)...")
        cm.Items.Add("미구현")
        cm.Font = New Font("맑은 고딕", 7)

        ' TreeListView1.ContextMenuStrip.ContextMenu.MenuItems.Add("수정")
        TreeListView1.ContextMenuStrip = cm
        DataGridView1.DataSource = _dtable
        AddHandler cm.ItemClicked, AddressOf contextmenu_click

    End Sub

#Region "메뉴 스트립"
    Private Shadows contextmenu As ContextMenuStrip
    Property _contextmenu As ContextMenuStrip
        Get
            Return contextmenu
        End Get
        Set(value As ContextMenuStrip)
            contextmenu = value
        End Set
    End Property
    Private Sub contextmenu_click(sender As Object, e As ToolStripItemClickedEventArgs)
        'Dim node As TreeListNode = e.Item
        'Dim temp As Double = CDbl(node.SubItems.Item(0).ToString)
        Dim imsbox As Qportals.Controls.InputBoxs = New Qportals.Controls.InputBoxs
        Dim msg As List(Of String) = New List(Of String)

        Select Case e.ClickedItem.Text
            Case "수정(Edit)..."

                If (TreeListView1.SelectedItems.Count > 0) Then
                    msg.Add(String.Format("수정할 내용을 입력 하세요. 
                                      'ex) 홍길동"))
                    msg.Add("내용 수정")
                    msg.Add(TreeListView1.SelectedItems.Item(0).SubItems.Item(0).ToString)

                    Dim result As String = imsbox.ShowInputBox(New String() {msg(0), msg(1), msg(2)}, Me.RectangleToScreen(Me.ClientRectangle))
                    Qportals.Debugging.Show("변경 후 " & result)

                    If Not (result Is Nothing) Then
                        TreeListView1.SelectedItems.Item(0).SubItems.Item(0).Text = result
                    End If
                End If



            Case "저장(미구현)"
                ' 추가 구현 사항
        End Select
    End Sub
#End Region

#Region "드래그 앤 드랍"
    Private Sub DragOver_OnPanel(sender As Object, e As Windows.Forms.DragEventArgs) Handles TreeListView1.DragEnter
        If (e.Data.GetDataPresent(Windows.DataFormats.FileDrop)) Then
            e.Effect = Windows.DragDropEffects.Copy
        Else
            e.Effect = Windows.DragDropEffects.None
        End If
    End Sub
    Private Sub DragEnter_OnPanel(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles TreeListView1.DragDrop
        Dim blCheck As Boolean = False       '# DragDrop 시 DragEventArgs를 통해 Data를 받아 온다. 
        Dim files() As String = e.Data.GetData(Forms.DataFormats.FileDrop)      '# Array 형식으로 저장 된 것을 난 파일 하나만 허용 할 거기 때문에 

        Dim dtExcel As Qportals.External_library.EPPlus = New Qportals.External_library.EPPlus
        Dim dt As DataTable = New DataTable
        TreeListView1.Nodes.Clear()

        Select Case files.Length
            Case 1
                dt = dtExcel.ReadAllxlsm_set_datatable(files(0), "Sheet1")
                Dim trv As Qportals.Controls.TreeListViewMaker = New Qportals.Controls.TreeListViewMaker With {
                        ._trv = TreeListView1
                    }

                If Not (dt Is Nothing) Then
                    trv.Make_Node_Project(dt, TreeListView1)
                End If

            Case Else
                Qportals.Debugging.Show("1개의 파일만 지원 합니다.", "(lee.sunbae@lgepartner.com)", MessageBoxButton.OK, Forms.MessageBoxIcon.Error)
        End Select

    End Sub
#End Region

#Region "<모델 검색!>"
    '** Enter 키 눌렀을 때
    Private Sub WhenEnterKeyDown(sender As Object, e As KeyEventArgs) _
            Handles txtPro.KeyDown, txtModel.KeyDown, txtStep.KeyDown, cbCompany.KeyDown
        If (e.KeyCode = Keys.Enter) Then
            SearchButtonClick(sender, e)
        End If
    End Sub

    '** 검색 버튼 눌렀을 때
    Private Sub SearchButtonClick(sender As Object, e As EventArgs) _
            Handles btnSearch.Click, btnS1.Click, btnS2.Click, btnS3.Click, btnS4.Click

        Dim srch As New Qportals.Level.SelectModeltoOtherForm
        Dim lst As New Qportals.Controls.ListViewMaker
        Dim openPopup As New Level_Control_TestItem_Assign_SelectModel

        ' ** ListView에 뿌리기 위해 DB연결 후 ... 
        srch._Connstring = "Server=10.169.88.40;Uid=rs_user;Pwd=lge1234;Database="
        srch._SchemaString = "rs2"
        srch._pro = txtPro.Text.ToString
        srch._grp = cbCompany.Text.ToString
        srch._mod = txtModel.Text.ToString
        srch._step = txtStep.Text.ToString

        ' ** Database 에 Datatable 완성 시키는 부분 입니다.
        srch.BuildListMain()

        ' ** 기존 리스트 지우고 ListView를 만든 다음 새로운 Form에 자료를 띄웁니다.
        openPopup.ListView1.Items.Clear()
        lst.BuildList(openPopup.ListView1, srch._dt, New Integer() {0, 1, 2, 3, 4, 5})
        openPopup.TopMost = True
        openPopup.ShowDialog()


    End Sub
#End Region

#Region "템플릿 다운로드"
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Template Download
        Dim iofile As Qportals.FileControl = New Qportals.FileControl

        'iofile.CopyFileTo("\\10.169.88.40\Q-Portal\resource\Assign_Template.xlsx", "")
        iofile.SaveFileDialog_sun_custom("\\10.169.88.40\Q-Portal\resource\Assign_Template.xlsx")


    End Sub
#End Region

#Region "<물음표에 마우스 올려놨을 때 보여지도록!>"
    Private Sub mouseOverGuide(sender As Object, e As EventArgs) Handles btnShowGuide.MouseHover
        Dim thrd As Thread = New Thread(AddressOf mouseAnimation)
        thrd.Start()
    End Sub
    Private Sub mouseLostFocus(sender As Object, e As EventArgs) Handles btnShowGuide.MouseLeave
        Dim thrd As Thread = New Thread(AddressOf mouseAnimation)
        Main_Panel.Controls.Remove(picbox)
        picbox = Nothing
    End Sub
    Public picbox As PictureBox = New PictureBox
    Private Sub mouseAnimation()

        picbox = New PictureBox
        With picbox
            .BorderStyle = BorderStyle.FixedSingle
            .Top = btnShowGuide.Top
            .Left = btnShowGuide.Left + 20
            .SizeMode = PictureBoxSizeMode.AutoSize
            .Size = New Drawing.Size(231, 223)
            .Name = "picbox"
            .Image = Image.FromFile("\\10.169.88.40\Q-Portal\resource\Upload_Guide.gif")
        End With

        If Main_Panel.InvokeRequired Then
            Main_Panel.Invoke(Sub() Main_Panel.Controls.Add(picbox))
            Main_Panel.Invoke(Sub() picbox.BringToFront())
            Thread.Sleep(15000)

        Else
            Main_Panel.Controls.Add(picbox)
            picbox.BringToFront()
            Thread.Sleep(15000)

        End If

    End Sub


#End Region

#Region "<이름 할당 부분에 ex가 있으면 지워줍니다."
    Private Sub removeText(sender As Object, e As EventArgs)
        'Dim txt As Qportals.Controls.TxtBoxStyle.RemoveExampleText = New Qportals.Controls.TxtBoxStyle.RemoveExampleText
        'txt.RemoveTextWhenEx(txtSearch)
    End Sub
#End Region

#Region "할당"
    Private _dt As DataTable
    Property dt As DataTable
        Get
            Return _dt
        End Get
        Set(value As DataTable)
            _dt = value
        End Set
    End Property
    Dim __User As String
    Property _User As String
        Get
            Return __User
        End Get
        Set(value As String)
            __User = value
        End Set
    End Property

    Private Function GetContacts(ByRef user As String) As DataTable
        Dim dbclass As Qportals.DbConnection.Mysql_Class = New Qportals.DbConnection.Mysql_Class
        Dim connString As String = "Server=10.169.88.40;Uid=rs_user;Pwd=lge1234;Database=" & "td_defect"
        Dim sql As String = "select `업체`, `부서`, `이름`, `직급` from td_defect.`Contacts_c` where "
        sql += "`이름` = '" & user & "'"

        Dim dt As DataTable = dbclass.Mysql_to_datatable(sql, connString)

        Return If(dt, Nothing)
    End Function

    Private Sub WhenEnterKey(sender As Object, e As KeyEventArgs)
        ' Delete 키 눌렀을 때 
        If (e.KeyCode = Keys.Enter) Then
            Btn_AssignClick(sender, e)
        End If
    End Sub

    ' OLD - 리스트 뷰 있을 때
    Private Sub Btn_AssignClick(sender As Object, e As EventArgs)

        '' ListView 읽어야 함.  
        'dt = GetContacts(txtSearch.Text.ToString)
        'Dim memselect As New Qportals.Controls.ListViewMaker

        '' Model 정보 관련  Project 정보를 DB에서 가져올지는 정해야 함.
        'If Not (dt Is Nothing) Then
        '    If (dt.Rows.Count > 1) Then '한 명이 아닌 경우
        '        Dim msform As New MemberSelect
        '        With MemberSelect.f.ListView1
        '            .View = View.Details
        '            memselect.BuildList(MemberSelect.f.ListView1, dt, New Integer() {0, 1, 2, 3})
        '            .Columns(1).Width = 80
        '            .FullRowSelect = True
        '            .MultiSelect = False
        '        End With
        '        msform.ShowDialog()
        '    Else
        '        'LstviewToDGView()
        '    End If
        'End If

    End Sub

    ' ** DataGridView에 할당 한 내용을 뿌려 줍니다.
    Private Sub LstviewToDGView()

        '    Dim User As String = txtSearch.Text.ToString()
        '    'Dim part As String = dt.Rows.Item("부서").ToString
        '    'Dim company As String = dt.Rows.Item("업체").ToString


        '    Dim drows As List(Of List(Of String)) = New List(Of List(Of String))
        '    Dim drow As List(Of String) = New List(Of String)
        '    With ListView1
        '        If Not (.SelectedItems Is Nothing) And Not (.Items.Count = 0) Then
        '            If (.SelectedItems.Count > 0) Then
        '                For i As Integer = .SelectedItems.Count - 1 To 0 Step -1
        '                    ' Temporate
        '                    drow.Add("MH43_POS") : drow.Add("내수") : drow.Add("LM-X625N_LGT") : drow.Add("VP01")
        '                    drow.Add(.SelectedItems(i).Text.ToString)
        '                    drow.Add(.SelectedItems(i).SubItems(1).Text.ToString())
        '                    drow.Add(.SelectedItems(i).SubItems(2).Text.ToString())
        '                    drow.Add(User)
        '                    'drow.Add(part)
        '                    'drow.Add(company)

        '                    'List<T>
        '                    drows.Add(drow)
        '                    drow = New List(Of String)
        '                Next

        '            Else
        '                ' 한 개 선택 했을 때 
        '                drow.Add("MH43_POS") : drow.Add("내수") : drow.Add("LM-X625N_LGT") : drow.Add("VP01")
        '                drow.Add(.SelectedItems(0).Text.ToString)
        '                drow.Add(.SelectedItems(0).SubItems(1).Text.ToString())
        '                drow.Add(.SelectedItems(0).SubItems(2).Text.ToString())
        '                drow.Add(User)
        '                drows.Add(drow)
        '                'drow.Add(part)
        '                'drow.Add(company)
        '                drow = New List(Of String)
        '            End If
        '        End If
        '    End With

        '    Dim row As DataRow
        '    For Each li As List(Of String) In drows
        '        row = _dtable.Rows.Add()
        '        row.ItemArray = li.ToArray()
        '        _dtable.AcceptChanges()
        '    Next

    End Sub

#End Region

#Region "TreeListView"
    Public drows As List(Of List(Of String)) = New List(Of List(Of String))
    Private treeListView As TreeListView
    Private trvListMaker As New Qportals.Controls.TreeListViewMaker
    Private Delegate Sub AddDgvDel(nodes As TreeNodeCollection)


#Region "트리리스트뷰 체크하면 올 체크 하는 거"
    Private Sub CheckedItems(sender As Object, e As ContainerListViewEventArgs) Handles TreeListView1.AfterCheckStateChanged
        'RemoveHandler TreeListView1.AfterCheckStateChanged, AddressOf checkValueChange
        'RemoveHandler TreeListView1.AfterCheckStateChanged, AddressOf CheckedItems

        Dim node As TreeListNode = e.Item
        trvListMaker.CheckBoxAll(node, node.Checked)

        'AddHandler TreeListView1.AfterCheckStateChanged, AddressOf CheckedItems
        'AddHandler TreeListView1.AfterCheckStateChanged, AddressOf checkValueChange
    End Sub
#End Region

#Region "데이터그리드뷰에 값 넣기 관련"

    Private Sub test()





    End Sub




    Private Sub AddDel_Main(sender As Object, e As EventArgs) Handles AddButton.Click, DeleteButton.Click
        Dim chkLists As List(Of Control) = New List(Of Control) From {
                txtProin,
                txtGroupin,
                txtModelin,
                txtStepin,
                txtStartin,
                txtEndin
            }
        Dim cnt As Integer = chkLists.Where(Function(x) x.Text = "").Count

        If (cnt > 0) Then
            Qportals.Debugging.Show("모델정보가 입력되지 않았습니다." & vbCrLf & "입력 후 다시 시도하세요",,, 16)
            Exit Sub
        End If
        'Dim lists As List(Of Control) = New List(Of Control) From {
        '    TextBox1', Textbox2, TextBox3, TextBox4
        '}
        'Dim cnt As Integer = lists.Select(Function(x) x.Text = "").Count


        If DirectCast(sender, Control).Name = "AddButton" Then
            checkValueChange()
        ElseIf DirectCast(sender, Control).Name = "DeleteButton" Then
            Del_main()
        End If

    End Sub


    Private Property Lists As List(Of List(Of String))
    Public isFinish As Boolean = False
    ' ** 트리뷰에 있는 값을 추가 합니다.
    Private Sub checkValueChange()
        DataGridView1.DataSource = _dtable
        DataGridView1.Refresh()
        Dim str1 As String = "", str2 As String = ""
        Dim isDupl As Boolean = False
        drows = New List(Of List(Of String))
        Add_Dgv(TreeListView1.Nodes)
        For Each li As List(Of String) In drows
            str1 = "" : isDupl = False

            For j As Integer = 0 To 10
                str1 += li(j)
            Next

            ' 중복인 건은 추가 안함
            For Each row As DataRow In _dtable.Rows
                str2 = ""
                For i As Integer = 0 To 10
                    str2 += row.Item(i).ToString
                Next
                If str1 = str2 Then
                    isDupl = True : Exit For
                End If
            Next

            ' 중복이 아닌 건 추가
            If isDupl = False Then
                Dim row As DataRow
                row = _dtable.Rows.Add()
                row.ItemArray = li.ToArray()
                _dtable.AcceptChanges()
            End If
        Next

    End Sub


    ' ** 함수1-1 : 추가할 List를 담는 함수 입니다.
    Private Sub Add_Dgv(nodes As TreeListNodeCollection)
        Dim drow As List(Of String) = New List(Of String)
        Dim temp As String : Dim temps() As String
        For Each node As TreeListNode In nodes
            If (node.Checked = True) And Not (node.ParentNode Is Nothing) Then
                temp = node.FullPath
                If temp.Split("\").Count = 2 Then
                    temps = temp.Split("\")
                    With drow
                        .Add(txtProin.Text.ToString) : .Add(txtGroupin.Text.ToString)
                        .Add(txtModelin.Text.ToString) : .Add(txtStepin.Text.ToString)
                        .Add(txtStartin.Text.ToString) : .Add(txtEndin.Text.ToString)
                        .Add(temps(0)) : .Add(temps(1))
                        .Add(node.SubItems.Item(0).ToString) : .Add("part") : .Add("업체명")
                    End With
                    drows.Add(drow)
                    drow = New List(Of String)
                End If
            End If
            Add_Dgv(node.Nodes)
        Next
    End Sub

    ' ** TreeListView 체크 해제 시 
    Private Sub Del_main() 'sender As Object, e As ContainerListViewCancelEventArgs) 'Handles TreeListView1.BeforeCheckStateChanged
        'RemoveHandler TreeListView1.BeforeCheckStateChanged, AddressOf Del_main
        ' CancelEvent의 BeforeCheck 이기 때문에 Checked = True 이면 해제 한 것.
        DataGridView1.Refresh()

        Dim str1 As String = "" : Dim str2 As String = ""
        'If (e.Item.Checked = True) Then
        drows = New List(Of List(Of String))
        If (_dtable.Rows.Count > 0) Then

            Del_Dgv(TreeListView1.Nodes)
            For Each li As List(Of String) In drows
                str1 = ""
                For i As Integer = 0 To 10 : str1 += li(i) : Next
                For j As Integer = _dtable.Rows.Count - 1 To 0 Step -1
                    For i As Integer = 0 To 10
                        str2 += _dtable.Rows(j)(i).ToString()
                    Next
                    If (str1 = str2) Then
                        _dtable.Rows(j).Delete()
                        _dtable.AcceptChanges()
                    End If
                    str2 = ""
                Next
                str2 = ""
            Next
            Lists = drows
            drows = New List(Of List(Of String))
        End If


        'End If

        'AddHandler TreeListView1.BeforeCheckStateChanged, AddressOf Del_main
    End Sub
    ' ** 함수 : 삭제할 List 담는 함수
    Private Sub Del_Dgv(nodes As TreeListNodeCollection)
        Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)
        Dim drow As List(Of String) = New List(Of String)
        Dim temp As String = "" : Dim temps() As String
        Dim str1 As String = ""
        Dim str2 As String = ""

        For Each node As TreeListNode In nodes
            If (node.Checked = True) Then
                temp = node.FullPath
                If temp.Split("\").Count = 2 Then
                    temps = temp.Split("\")
                    With drow
                        .Add(txtProin.Text.ToString) : .Add(txtGroupin.Text.ToString)
                        .Add(txtModelin.Text.ToString) : .Add(txtStepin.Text.ToString)
                        .Add(txtStartin.Text.ToString) : .Add(txtEndin.Text.ToString)
                        .Add(temps(0)) : .Add(temps(1))
                        .Add(node.SubItems.Item(0).ToString) : .Add("part") : .Add("업체명")
                    End With
                    drows.Add(drow)
                    drow = New List(Of String)
                End If

            End If
            Del_Dgv(node.Nodes)
        Next

    End Sub
#End Region
    Private Sub checkValueChange_temp() 'sender As Object, e As ContainerListViewEventArgs)
        DataGridView1.DataSource = _dtable
        'DataGridView1.Refresh()
        'Dim str1 As String = "", str2 As String = ""
        'Dim dt As DataTable = New DataTable
        'If (TreeListView1.CheckedItems.Count > 0) Then
        '    Add_Dgv(TreeListView1.Nodes)

        '    For Each li As List(Of String) In drows
        '        Dim row As DataRow
        '        row = _dtable.Rows.Add()
        '        row.ItemArray = li.ToArray()
        '        dt.AcceptChanges()
        '    Next

        'For i As Integer = 0 To _dtable.Rows.Count - 1
        '    str1 = ""
        '    For j As Integer = 0 To 10 : str1 += _dtable.Rows(i)(j) : Next

        '    For Each li As List(Of String) In drows
        '        For q As Integer = 0 To 10 : str2 += _dtable.Rows(i)(q).ToString : Next
        '        If (str1 = str2) Then
        '            Dim row As DataRow
        '            row = _dtable.Rows.Add()
        '            row.ItemArray = li.ToArray()
        '            dt.AcceptChanges()
        '        End If
        '        str2 = ""

        '    Next
        'Next
        'End If
        Add_Dgv(TreeListView1.Nodes)
        Dim row As DataRow
        For Each li As List(Of String) In drows
            row = _dtable.Rows.Add()
            row.ItemArray = li.ToArray()
            _dtable.AcceptChanges()
        Next
        Lists = drows
        drows = New List(Of List(Of String))

        '_dtable = _dtable.DefaultView.ToTable(True, "Project", "GroupName", "Model", "Step", "Start_Date", "End Date", "TCType", "TCName", "Tester", "Part", "업체명")
    End Sub
    Private Sub Add_Dgv_temp(nodes As TreeListNodeCollection)

        Dim drow As List(Of String) = New List(Of String)
        Dim temp As String : Dim temps() As String

        For Each node As TreeListNode In nodes
            If (node.Checked = True) And Not (node.ParentNode Is Nothing) Then
                temp = node.FullPath
                If temp.Split("\").Count = 2 Then
                    temps = temp.Split("\")
                    With drow
                        .Add(txtProin.Text.ToString) : .Add(txtGroupin.Text.ToString)
                        .Add(txtModelin.Text.ToString) : .Add(txtStepin.Text.ToString)
                        .Add(txtStartin.Text.ToString) : .Add(txtEndin.Text.ToString)
                        .Add(temps(0)) : .Add(temps(1))
                        .Add(node.SubItems.Item(0).ToString) : .Add("part") : .Add("업체명")
                    End With
                    drows.Add(drow)
                    drow = New List(Of String)
                End If
            End If
            Add_Dgv(node.Nodes)
        Next

    End Sub

    Private Sub Del_Dgv_temp()
        Dim dt As DataTable = TryCast(DataGridView1.DataSource, DataTable)

        Dim str1 As String = ""
        Dim str2 As String = ""

        If Not (dt Is Nothing) And Not (Lists Is Nothing) Then
            For Each li As List(Of String) In Lists
                For i As Integer = 0 To 11 : str1 += li(i) : Next

                For Each dr As DataRow In dt.Rows
                    For i As Integer = 0 To 11 : str2 += dr.Item(i).ToString : Next

                    If (str1 = str2) Then
                        dr.Delete()
                    End If

                Next

            Next
        End If

    End Sub
#End Region



#Region "<Summary> 접기/펴기 </Summary>"
    Private Sub btnExpandAll_true_Click(sender As Object, e As EventArgs) Handles btnExpandAll_true.Click
        TreeListView1.ExpandAll()
    End Sub

    Private Sub btnExpandAll_false_Click(sender As Object, e As EventArgs) Handles btnExpandAll_false.Click
        TreeListView1.CollapseAll()
    End Sub
#End Region

#Region "<Summary> Copy & Paste 할 때 </Summary>"
    Private Sub CopyPaste(sender As Object, e As KeyEventArgs) Handles TreeListView1.KeyDown
        Dim trv As Qportals.Controls.TreeListViewMaker = New Qportals.Controls.TreeListViewMaker With {
                ._trv = TreeListView1
            }

        Dim a As DataTable

        If (e.Control = True And e.KeyCode = Keys.V) Then
            Label3.Visible = False
            a = PasteData(Windows.Clipboard.GetText())

            If Not (a Is Nothing) Then
                Dim dv As DataView
                Try
                    lc._dtable = a
                    dv = a.DefaultView
                    dv.Sort = "[T/C Type], [이름]  ASC"
                    a = dv.ToTable
                    trv.Make_Node_Project(a, TreeListView1)
                    'CheckBoxsVisible()
                Catch ex As Exception
                    Qportals.Debugging.Show(ex.Message)
                End Try

            End If
        End If

    End Sub

    Private Sub WhenClickCheckBox()
        ' check box 클릭 시 체크 된 항목들을 반복 하면서 
        ' datagridview에 넘김 



    End Sub
    Private Sub CheckBoxsVisible()

        For Each child As TreeListNode In TreeListView1.Nodes
            If (child.ParentNode Is Nothing) Then
                child.CheckBoxVisible = False
            End If
        Next

    End Sub

    Private Function PasteData(ByRef pClipboard As String) As DataTable
        Dim table As List(Of List(Of String)) = New List(Of List(Of String))
        Dim importText As String = pClipboard
        Dim dt As DataTable = New DataTable
        Dim dCol As DataColumn = New DataColumn

        Try
            ' ** 클립보드의 
            importText = importText.Replace(vbLf, "")

            Dim lines() As String = importText.Split(vbCrLf)

            Dim strException() As String

            For Each strX As String In lines
                strException = strX.Split(vbTab)
                strX = strX.Replace(Environment.NewLine, String.Empty)
            Next

            For i As Integer = 0 To lines.Length - 1

                If (String.IsNullOrEmpty(lines(i))) Then
                    Exit For
                End If

                Dim cellList As New List(Of String)
                Dim cells() As String = lines(i).Split(vbTab)

                For Each str As String In cells
                    str = str.Replace(Environment.NewLine, String.Empty)
                Next

                cellList.AddRange(cells) : table.Add(cellList)

            Next

            For i As Integer = 0 To table.Item(0).Count - 1
                dCol = New DataColumn : dCol.DataType = GetType(String)
                If (table.Item(0).Item(1).ToString.Contains("T/C Type") = True) Then
                    With dCol
                        .ColumnName = table.Item(0).Item(i).ToString
                        .Caption = table.Item(0).Item(i).ToString
                        dt.Columns.Add(dCol)
                    End With
                Else
                    Exit For
                End If
            Next

            Dim c1 As DataColumn = New DataColumn
            Dim c2 As DataColumn = New DataColumn
            Dim c3 As DataColumn = New DataColumn
            Dim dcols As DataColumn() = New DataColumn() {c1, c2, c3}

            Dim strColumns() As String = {"T/C Type", "Test Item", "이름"}
            Dim a As Integer = 0
            For Each col As DataColumn In dcols
                col.DataType = GetType(String)
                col.ColumnName = strColumns(a).ToString
                col.Caption = strColumns(a).ToString
                dt.Columns.Add(col)
                a += 1
            Next

            Dim chk As Boolean = False
            For Each s As List(Of String) In table
                If (s.Count > 1) And (chk = True) Then
                    dt.Rows.Add(s.ToArray)
                End If
                chk = True
            Next
        Catch ex As Exception
            Qportals.Debugging.Show(ex.Message & " (?) 가이드를 확인 해주세요.")
        End Try

        Return If(dt Is Nothing, Nothing, dt)

    End Function
#End Region

#Region "슬라이드 메뉴"
    Public Sub SlidePanel(sender As Object, e As EventArgs) Handles slideButton.Click
        Dim CustomPanel As Qportals.Controls.SlidePanel = New Qportals.Controls.SlidePanel With {
            ._slideButton = Slide_btnPanel,
            ._slidePanel = Slide_panel
            }
        Dim trd1 As Thread = New Thread(Sub() CustomPanel.Panel_OpenSlide(0, 271))
        Dim trd2 As Thread = New Thread(Sub() CustomPanel.Panel_CloseSlide(271, 0))
        If Slide_panel.Height > 0 Then
            trd2.Start()            'Close
        Else
            trd1.Start()            'Open
        End If
    End Sub

    Private Sub Btn_addProject_Click(sender As Object, e As EventArgs) Handles btn_addProject.Click
        Dim lv_add As New Level_Control_AddModel
        lv_add.Show()
    End Sub





#End Region

End Class




