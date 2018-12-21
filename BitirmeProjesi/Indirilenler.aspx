<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Indirilenler.aspx.cs" Inherits="BitirmeProjesi.Indırılenler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>İndirilenler</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="breadcrumbs">
        <div class="breadcrumbs-inner">
            <div class="row m-0">
                <div class="col-sm-8">
                    <div class="page-header float-right">
                        <div class="page-title">
                            <ol class="breadcrumb text-right">
                                <li>İndirilenler</li>
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
                                                <asp:LinkButton ID="lnk_but_klasor" runat="server" CommandName="KlasorLink" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"klasor_indirilme_id") %>'>
                                                    <i class="fa fa-folder-open"></i>&nbsp;<%#DataBinder.Eval(Container.DataItem,"indirilen_klasor_adi") %>
                                                </asp:LinkButton>
                                            </div>
                                            <p></p>
                                            <div class="stat-heading" style="font-size: 13px;">Sahibi:@<%#KullaniciAdiGetir(DataBinder.Eval(Container.DataItem,"sahip_kullanici_id")) %></div>
                                            

                                            <div class="icon-container" style="float:left; ">
                                                <asp:LinkButton ID="lnk_but_klasorsil" runat="server" CommandName="IndirilenKlasorSil" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"klasor_indirilme_id") %>'>
                                                    <span class="ti-trash"></span>
                                                </asp:LinkButton>

                                            </div>
                                            <div class="stat-heading" style="float:right;font-size: 13px; width:100px; margin-top:-10px; margin-right:10px;"><%#DataBinder.Eval(Container.DataItem,"indirilme_tarihi") %></div>
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
                                                <%#DataBinder.Eval(Container.DataItem,"indirilen_dosya_adi") %>
                                            </div>
                                            <p></p>
                                            <div class="stat-heading" style="font-size: 13px;">Sahibi:@<%#KullaniciAdiGetir(DataBinder.Eval(Container.DataItem,"sahip_kullanici_id")) %></div>
                                            
                                            
                                            <p></p>
                                            <div class="icon-container" style="float: left;">
                                                <asp:LinkButton ID="lnk_but_dosyasil" runat="server" CommandName="IndirilenDosyaSil" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"dosya_indirilme_id") %>'>
                                                    <span class="ti-trash"></span>
                                                </asp:LinkButton>
                                            </div>
                                            <div class="stat-heading" style="float:right;font-size: 13px; width:100px; margin-top:-10px; margin-right:10px;"><%#DataBinder.Eval(Container.DataItem,"indirilme_tarihi") %></div>
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
