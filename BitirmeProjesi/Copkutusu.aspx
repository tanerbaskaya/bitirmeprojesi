<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Copkutusu.aspx.cs" Inherits="BitirmeProjesi.Copkutusu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="breadcrumbs">
        <div class="breadcrumbs-inner">
            <div class="row m-0">
                <div class="col-sm-4">
                </div>
                <div class="col-sm-8">
                    <div class="page-header float-right">
                        <div class="page-title">
                            <ol class="breadcrumb text-right" style="float:right;">
                                <li><a href="Copkutusu.aspx">Çöp Kutusu</a><%for (int i = listDizinId.Count-1; i>=0  ; i--) { Response.Write("<a href ='Copkutusu.aspx?sid=" + listDizinId[i] +"'>/" + listDizinAdi[i] + "</a>"); } %></li>
                            </ol>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <!-- Animated -->
        <div class="animated fadeIn">
            <!-- Widgets  -->
            <p>Klasörler</p>
            <hr />
            <div class="row">

                <asp:Repeater ID="repeater_Klasor" runat="server" OnItemCommand="repeater_Klasor_ItemCommand">
                    <ItemTemplate>
                        <div class="col-lg-3 col-md-6">
                            <div class="card">
                                <div class="card-body">
                                    <div class="stat-widget-five">
                                        <div class="text-left dib">
                                            <div class="stat-text" style="font-size: 15px">
                                                <asp:LinkButton ID="lnk_but_klasor" runat="server" CommandName="KlasorLink" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"klasor_id") %>'>
                                                    <i class="fa fa-folder-open"></i>&nbsp;<%#DataBinder.Eval(Container.DataItem,"klasor_adi") %>
                                                </asp:LinkButton>
                                            </div>
                                            <p></p>
                                            <div class="icon-container" style="float:left; width:150px; ">
                                                <%if (Request.QueryString["sid"] == null)
                                                    { %>
                                                <asp:LinkButton ID="lnk_but_klasorgeriyukle" runat="server" CommandName="KlasorGeriYukle" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"klasor_id") %>'>
                                                    <span class="ti-arrow-left"></span><span class="icon-name">Geri Yükle</span>
                                                </asp:LinkButton>
                                                <%} %>
                                            </div>

                                            <div class="icon-container" style="float:left; width:30px;">
                                                <asp:LinkButton ID="lnk_but_klasorsil" runat="server" CommandName="KlasorSil" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"klasor_id") %>'>
                                                    <span class="ti-trash"></span>
                                                </asp:LinkButton>
                                            </div>
                                            
                                            
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <p>Dosyalar</p>
            <hr />
            <div class="row">
                <asp:Repeater ID="repeater_Dosya" runat="server" OnItemCommand="repeater_Dosya_ItemCommand">
                    <ItemTemplate>
                        <div class="col-lg-3 col-md-6">
                            <div class="card">
                                <div class="card-body">
                                    <div class="stat-widget-five">
                                        <div class="text-left dib">
                                            <div class="stat-text" style="font-size: 15px">
                                                <%#DataBinder.Eval(Container.DataItem,"dosya_adi") %>
                                            </div>
                                            <p></p>
                                            <div class="icon-container" style="float:left; width:150px; ">
                                                <%if (Request.QueryString["sid"] == null)
                                                    { %>
                                                <asp:LinkButton ID="lnk_but_dosyageriyukle" runat="server" CommandName="DosyaGeriYukle" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"dosya_id") %>'>
                                                    <span class="ti-arrow-left"></span><span class="icon-name">Geri Yükle</span>
                                                </asp:LinkButton>
                                                <%} %>
                                            </div>
                                            <div class="icon-container" style="float: left; width:30px;">
                                                <asp:LinkButton ID="lnk_but_dosyasil" runat="server" CommandName="DosyaSil" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"dosya_id") %>'>
                                                    <span class="ti-trash"></span>
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <!-- .animated -->

    </div>
</asp:Content>
