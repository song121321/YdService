namespace YdService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.YdServiceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.YdService = new System.ServiceProcess.ServiceInstaller();
            // 
            // YdServiceProcessInstaller1
            // 
            this.YdServiceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.YdServiceProcessInstaller1.Password = null;
            this.YdServiceProcessInstaller1.Username = null;
            // 
            // YdService
            // 
            this.YdService.ServiceName = "YdService";
            this.YdService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.YdServiceProcessInstaller1,
            this.YdService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller YdServiceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller YdService;
    }
}