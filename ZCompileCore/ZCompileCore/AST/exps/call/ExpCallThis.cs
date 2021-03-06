﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using ZCompileCore.Contexts;
using ZCompileCore.Lex;
using ZCompileCore.Tools;
using ZLangRT;
using ZCompileDesc.Descriptions;
using ZCompileKit.Tools;
using System.Reflection;

namespace ZCompileCore.AST
{
    public class ExpCallThis : ExpCallAnalyedBase
    {
        ZMethodDesc SearchedProcDesc;

        public ExpCallThis(ContextExp context, ZCallDesc expProcDesc, ZMethodDesc searchedMethod, Exp srcExp, List<Exp> argExps)
        {
            this.ExpContext = context;
            this.ExpProcDesc = expProcDesc;
            this.SearchedProcDesc = searchedMethod;
            this.SrcExp = srcExp;
            this.ArgExps = argExps;
        }

        public override Exp Analy( )
        {
            RetType = this.SearchedProcDesc.ZMethod.RetZType;
            return this;
        }

        public override void Emit()
        {
            EmitSubject();
            EmitArgsThis(SearchedProcDesc, ArgExps);
            EmitHelper.CallDynamic(IL, SearchedProcDesc.ZMethod.SharpMethod);
            EmitConv();
        }

        protected void EmitArgsThis(ZMethodDesc zdesc, List<Exp> expArgs)
        {
            var paramArr = zdesc.DefArgs.ToArray();
            List<Exp> expArgsNew = CallAjuster.AdjustExps(paramArr, expArgs);
            EmitArgsExp(paramArr, expArgs.ToArray());
        }

        protected void EmitArgsExp(ZParam[] paramInfos, Exp[] args)
        {
            var size = paramInfos.Length;

            for (int i = 0; i < size; i++)
            {
                Exp argExp = args[i];
                var parameter = paramInfos[i];
                argExp.RequireType = parameter.ZParamType;
                argExp.Emit();
            }
        }

        private void EmitSubject()
        {
            if(IsNested && this.ClassContext.NestedOutFieldSymbol!=null)
            {
                IL.Emit(OpCodes.Ldarg_0);
                EmitHelper.LoadField(IL, this.ClassContext.NestedOutFieldSymbol.Field);
            }
            else if (SearchedProcDesc.ZMethod.IsStatic==false)
            {
                IL.Emit(OpCodes.Ldarg_0);
            }
            else
            {
                //...
            }
        }

    }
}
