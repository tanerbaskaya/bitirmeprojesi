<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Anasayfa.aspx.cs" Inherits="BitirmeProjesi.Anasayfa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Anasayfa</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="breadcrumbs">
        <div class="breadcrumbs-inner">
            <div class="row m-0">
                <div class="col-sm-4">
                    <div class="page-header float-left">
                        <button type="button" class="btn btn-success mb-1" data-toggle="modal" data-target="#smallmodal" style="float: left; margin-top: 7px; margin-right: 3px;">
                            Klasör Oluştur
                        </button>

                        <button type="button" class="btn btn-success mb-1" data-toggle="modal" data-target="#smallmodal2" style="margin-top: 7px;">
                            Dosya Yükle
                        </button>
                    </div>
                </div>
                <div class="col-sm-8">
                    <div class="page-header float-right">
                        <div class="page-title">
                            <ol class="breadcrumb text-right">
                                <li>Tüm Dosyalarım</li>
                            </ol>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="smallmodal" tabindex="-1" role="dialog" aria-labelledby="smallmodalLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="smallmodalLabel">Klasör Oluştur</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <p>
                        <asp:TextBox ID="txt_Klasorolustur" runat="server" class="form-control" PlaceHolder="Klasör Adı" ValidationGroup="KlasorOlustur"></asp:TextBox>
                    </p>
                    <p>
                        <asp:TextBox ID="txt_KlasorAciklama" class="form-control" runat="server" PlaceHolder="Klasör Açıklaması(Zorunlu Değil)" TextMode="MultiLine"></asp:TextBox>
                    </p>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Klasor Alanını Boş Bırakmayınız" ValidationGroup="KlasorOlustur" ControlToValidate="txt_klasorolustur"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="CustomValidator" ValidationGroup="KlasorOlustur"></asp:CustomValidator>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Kapat</button>
                    <asp:Button ID="btn_Klasorolustur" runat="server" Text="Oluştur" class="btn btn-primary" OnClick="btn_Klasorolustur_Click" ValidationGroup="KlasorOlustur" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="smallmodal2" tabindex="-1" role="dialog" aria-labelledby="smallmodalLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="smallmodalLabel2">Dosya Yükle</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <p>
                        <asp:FileUpload ID="fucDosyaYukle" runat="server" class="form-control" ValidationGroup="DosyaYukle" />
                    </p>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="DosyaYukle" ControlToValidate="fucDosyaYukle"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="CustomValidator2" runat="server" ErrorMessage="CustomValidator" ValidationGroup="DosyaYukle"></asp:CustomValidator>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Kapat</button>
                    <asp:Button ID="btn_DosyaYukle" runat="server" Text="Yükle" class="btn btn-primary" ValidationGroup="DosyaYukle" OnClick="btn_DosyaYukle_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="smallmodal3" tabindex="-1" role="dialog" aria-labelledby="smallmodalLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="smallmodalLabel3">Klasör Açıklaması</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lbl_KlasorAciklama" runat="server" Text="Label"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Kapat</button>
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
                                            <div class="stat-heading" style="font-size: 13px"><%#DataBinder.Eval(Container.DataItem,"olusturma_tarihi") %></div>
                                            <p></p>
                                            <div style="font-size: 14px;">
                                                <asp:LinkButton ID="lnk_but_KlasorPaylasim" CommandName="KlasorPaylasim" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"klasor_id")%>' runat="server">
											<i class="fa fa-link"></i>&nbsp;<%#PaylasimDurumuGetir(DataBinder.Eval(Container.DataItem,"paylasim_durumu"))%></a>
                                                </asp:LinkButton>
                                            </div>
                                            <asp:LinkButton ID="LinkButton1" runat="server" Visible='<%#PaylasimDurumuKontrol(DataBinder.Eval(Container.DataItem,"paylasim_durumu"))%>'>
                                            <div class="icon-container" style="float: left; width: 40px;">
                                                <a href='Paylasim.aspx?kpid=<%#DataBinder.Eval(Container.DataItem,"paylasim_kodu") %>'><span class="ti-link"></span></a>
                                            </div>
                                            </asp:LinkButton>
                                            <div class="icon-container" style="float: left; width: 40px;">
                                                <asp:LinkButton ID="lnk_but_klasorsil" runat="server" CommandName="KlasorSil" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"klasor_id") %>'>
                                                    <span class="ti-trash"></span>
                                                </asp:LinkButton>

                                            </div>
                                            <div class="icon-container" style="float: left; width: 40px;">
                                                <asp:LinkButton ID="LinkButton2" runat="server" CommandName="KlasorIndir" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"klasor_id") %>'>
                                                    <span class="ti-download"></span>
                                                </asp:LinkButton>
                                            </div>

                                            <div class="dropdown float-right">
                                                <div class="icon-container" style="float: left; width: 40px;">
                                                    <asp:Panel ID="Panel1" runat="server" Visible='<%#KlasorYorumMevcutmu(DataBinder.Eval(Container.DataItem,"klasor_aciklama"))%>'>
                                                        <span class="ti-comment" id="dropdownMenuButton1" data-toggle="dropdown"></span>

                                                        <div class="dropdown-menu" aria-labelledby="dropdownMenuButton1">
                                                            <p>Klasör Açıklaması</p>
                                                            <%#DataBinder.Eval(Container.DataItem,"klasor_aciklama")%>
                                                        </div>
                                                    </asp:Panel>
                                                </div>
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
                                                &nbsp;<%#DataBinder.Eval(Container.DataItem,"dosya_adi") %>
                                            </div>
                                            <p></p>
                                            <div class="stat-heading" style="font-size: 13px"><%#DataBinder.Eval(Container.DataItem,"olusturma_tarihi") %></div>
                                            <p></p>
                                            <div style="font-size: 14px;">
                                                <asp:LinkButton ID="lnk_but_DosyaPaylasim" CommandName="DosyaPaylasim" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"dosya_id") %>' runat="server">
											<i class="fa fa-link"></i>&nbsp;<%#PaylasimDurumuGetir(DataBinder.Eval(Container.DataItem,"paylasim_durumu"))%></a>
                                                </asp:LinkButton>
                                            </div>
                                            <asp:LinkButton ID="LinkButton1" runat="server" Visible='<%#PaylasimDurumuKontrol(DataBinder.Eval(Container.DataItem,"paylasim_durumu"))%>'>
                                            <div class="icon-container" style="float: left; width: 40px;">
                                                <a href='Paylasim.aspx?dpid=<%#DataBinder.Eval(Container.DataItem,"paylasim_kodu") %>'><span class="ti-link"></span></a>
                                            </div>
                                            </asp:LinkButton>

                                            <div class="icon-container" style="float: left; width: 40px;">
                                                <asp:LinkButton ID="lnk_but_dosyasil" runat="server" CommandName="DosyaSil" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"dosya_id") %>'>
                                                    <span class="ti-trash"></span>
                                                </asp:LinkButton>
                                            </div>
                                            <div class="icon-container" style="float: left; width: 40px;">
                                                <asp:LinkButton ID="lnk_but_dosyaindir" runat="server" CommandName="DosyaIndir" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"dosya_id") %>'>
												<span class="ti-download"></span>
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
