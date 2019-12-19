using System;
using System.Collections.Generic;
using System.Text;

namespace Basestation.Common
{
    public static class StaticUtils
    {
        //System config paths
        public static string SystemConfigRelativePath => "init_conf.yml";
        public static string SystemConfigDevelopmentRelativePath => "../Deployment/init_conf_dev.yml";
        public static string SystemConfigDevLauncherRelativePath => "init_conf_dev.yml";

        //Id
        public static string LocalIdPath => "id";
        public static string SystemHealthMonitorId => "5D6F284A-BE57-4082-B012-3145933B6C8F";
        public static string MobileAppCommunicationId => "62BD2F32-F072-4956-989F-1BA83C9FC18A";
        public static string DataAcquisitionShimmerId => "4CC24444-5870-4EB6-B4F2-B8120D97D785";
        public static string LocalHealthEvaluationHeartrateId => "8A5E9A22-4B51-4057-9488-296A58A63123";
        public static string LocalHealthEvaluationArythmiaId => "F312B929-DE5E-4477-805A-9FECCCEA6FBA";
        public static string LocalHealthEvaluationCardiacId => "0B9BF565-3620-4CB9-A183-A00904DA4CA1";


    }
}