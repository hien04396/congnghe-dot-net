package tests.common;

import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.api.extension.ExtensionContext;
import org.junit.jupiter.api.extension.TestWatcher;
import org.openqa.selenium.WebDriver;

@ExtendWith(InKetQua.BaoCao.class)
public abstract class InKetQua {

    protected WebDriver driver;

    @BeforeEach
    public void batDauTest() {
        System.out.println("========== BẮT ĐẦU TEST ==========");
        driver = CauHinh.moChrome();
        sauKhiMoChrome();
    }

    protected void sauKhiMoChrome() {
    }

    @AfterEach
    public void ketThucTest() {
        System.out.println("========== KẾT THÚC TEST ==========");
        if (driver != null) {
            driver.quit();
        }
    }

    public static class BaoCao implements TestWatcher {

        @Override
        public void testSuccessful(ExtensionContext context) {
            inRa(context.getDisplayName(), "PASSED");
        }

        @Override
        public void testFailed(ExtensionContext context, Throwable cause) {
            inRa(context.getDisplayName(), "FAILED");
        }

        private void inRa(String tenTest, String ketQua) {
            System.out.println();
            System.out.println("========================================");
            System.out.println("TEST: " + tenTest);
            System.out.println("RESULT: " + ketQua);
            System.out.println("========================================");
        }
    }
}
