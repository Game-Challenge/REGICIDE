package response

import (
	"net/http"

	"github.com/gin-gonic/gin"
)

// 返回数据规范
// {
// 	code : 200,
// 	data : xxx,
// 	msg  : xxx,
// }

func Response(ctx *gin.Context, httpStats int, code int, data gin.H, msg string) {
	ctx.JSON(httpStats, gin.H{
		"code": code,
		"data": data,
		"msg":  msg,
	})
}

func Success(ctx *gin.Context, data gin.H, msg string) {
	Response(ctx, http.StatusOK, 200, data, msg)
}

func Failed(ctx *gin.Context, data gin.H, msg string) {
	Response(ctx, http.StatusOK, 400, data, msg)
}
