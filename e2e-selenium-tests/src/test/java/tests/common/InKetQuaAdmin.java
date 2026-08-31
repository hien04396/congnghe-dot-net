package tests.common;

public abstract class InKetQuaAdmin extends InKetQua {

    @Override
    protected void sauKhiMoChrome() {
        CauHinh.dangNhapAdmin(driver);
    }
}
