function fn1()
{
   console.log("a.js")
}
function fn2() {
   console.log("fn2")
}

exports.fn2=fn2;
export default { "fn1": fn1}



 public static void ToExceptinonLog(this Exception ex)
{
   LogFactory.GetLogger(typeof (Ext)).Error("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
   string msg = ex.ToJson();
   LogFactory.GetLogger(typeof (Ext)).Error(msg);
   if (ex.InnerException != null) {
      msg = ex.InnerException.ToJson();
      LogFactory.GetLogger(typeof (Ext)).Error(msg);
   }
   LogFactory.GetLogger(typeof (Ext)).Error("=================================================================================");
}