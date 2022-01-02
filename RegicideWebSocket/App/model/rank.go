package model

import "github.com/jinzhu/gorm"

type Rank struct {
	gorm.Model
	Name     string `gorm":type:varchar(20);not null"`
	Username string `gorm:varchar(11);not null`
	Roleid   int    `gorm:int(11);not null`
	Rumen    int    `gorm:int(11);not null`
	Easy     int    `gorm:int(11);not null`
	Normal   int    `gorm:int(11);not null`
	Hard     int    `gorm:int(11);not null`
	VeryHard int    `gorm:int(11);not null`
}
