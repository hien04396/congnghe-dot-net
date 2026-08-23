package tests.common;

/**
 * Class cha cho test trang Admin (đã đăng nhập).
 * LoginTest và DashboardTest không dùng class này vì cần tự thao tác login.
 */
public abstract class InKetQuaAdmin extends InKetQua {

    @Override
    protected void sauKhiMoChrome() {
        CauHinh.dangNhapAdmin(driver);
    }
}
