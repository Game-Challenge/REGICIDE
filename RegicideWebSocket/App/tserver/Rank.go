package tserver

import (
	"github.com/gin-gonic/gin"
)

func GetRankList(c *gin.Context) {
	//升级get请求为webSocket协议
	ws, err := upGrader.Upgrade(c.Writer, c.Request, nil)
	if err != nil {
		return
	}

	connid++
	uniid := connid
	ConnMap[connid] = ws
	go handleClient(ws, uniid)
}

// func Resigter(ctx *gin.Context) {
// 	DB := common.GetDB()
// 	//获取参数
// 	name := ctx.PostForm("name")
// 	telephone := ctx.PostForm("telephone")
// 	password := ctx.PostForm("password")
// 	//数据验证
// 	if len(name) >= 15 {
// 		ctx.JSON(http.StatusUnprocessableEntity, gin.H{"code": 422, "msg": "用户名不能大于15位数"})
// 		return
// 	}
// 	if len(name) == 0 {
// 		name = util.RandomString(10)
// 	}
// 	log.Println(name)
// 	if len(telephone) != 11 {
// 		response.Response(ctx, http.StatusUnprocessableEntity, 422, nil, "手机号必须为11位数")
// 		return
// 	}
// 	if len(password) < 6 {
// 		response.Response(ctx, http.StatusUnprocessableEntity, 422, nil, "密码不能少于6位数")
// 		return
// 	}
// 	//判断手机号是否存在
// 	if isTelephoneExist(DB, telephone) {
// 		response.Response(ctx, http.StatusUnprocessableEntity, 422, nil, "用户已存在")
// 		return
// 	}
// 	//创建用户
// 	//加密密码
// 	hasedPassword, err := bcrypt.GenerateFromPassword([]byte(password), bcrypt.DefaultCost)
// 	if err != nil {
// 		response.Response(ctx, http.StatusInternalServerError, 500, nil, "密码加密错误")
// 		return
// 	}
// 	newUser := model.User{
// 		Name:      name,
// 		Telephone: telephone,
// 		Password:  string(hasedPassword),
// 		// Password:  password,
// 	}
// 	DB.Create(&newUser)

// 	//发放token
// 	token, err := common.ReleaseToken(newUser)
// 	if err != nil {
// 		ctx.JSON(http.StatusInternalServerError, gin.H{"code": 500, "msg": "系统异常"})
// 		log.Printf("token generated error: %v", err)
// 		return
// 	}
// 	//返回结果
// 	response.Success(ctx, gin.H{"token": token}, "Resigter Success")
// }

// func Login(ctx *gin.Context) {
// 	DB := common.GetDB()
// 	//使用map 获取请求参数	方式1
// 	var requestMap = make(map[string]string)
// 	json.NewDecoder(ctx.Request.Body).Decode(&requestMap)

// 	//使用结构体 获取请求参数 方式2
// 	// var requestUser = model.User{}
// 	// json.NewDecoder(ctx.Request.Body).Decode(&requestUser) //方式一
// 	// ctx.Bind(&requestUser) //方式二

// 	//获取参数
// 	telephone := requestMap["telephone"]
// 	password := requestMap["password"]
// 	//数据验证
// 	if len(telephone) != 11 {
// 		response.Response(ctx, http.StatusUnprocessableEntity, 422, nil, "手机号必须为11位数")
// 		return
// 	}
// 	if len(password) < 6 {
// 		response.Response(ctx, http.StatusUnprocessableEntity, 422, nil, "密码不能少于6位数")
// 		return
// 	}
// 	//判断手机号是否存在
// 	var user model.User
// 	DB.Where("telephone = ?", telephone).First(&user)
// 	if user.ID == 0 {
// 		response.Response(ctx, http.StatusUnprocessableEntity, 422, nil, "用户不存在")
// 		return
// 	}

// 	//判断账号密码正确与否
// 	if err := bcrypt.CompareHashAndPassword([]byte(user.Password), []byte(password)); err != nil {
// 		ctx.JSON(http.StatusBadRequest, gin.H{"code": 400, "msg": "密码错误"})
// 		return
// 	}
// 	//发放token
// 	token, err := common.ReleaseToken(user)
// 	if err != nil {
// 		ctx.JSON(http.StatusInternalServerError, gin.H{"code": 500, "msg": "系统异常"})
// 		log.Printf("token generated error: %v", err)
// 		return
// 	}
// 	//返回结果
// 	response.Success(ctx, gin.H{"token": token}, "登录成功")
// 	// ctx.JSON(200, gin.H{
// 	// 	"code": 200,
// 	// 	"data": gin.H{
// 	// 		"token": token,
// 	// 	},
// 	// 	"msg": "登录成功",
// 	// })
// }
