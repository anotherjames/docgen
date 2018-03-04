package main

import (
	"fmt"

	"github.com/gohugoio/hugo/deps"
	"github.com/gohugoio/hugo/hugofs"
	"github.com/gohugoio/hugo/hugolib"
)

func main() {
	var cfg = &deps.DepsCfg{}
	osFs := hugofs.Os

	config, err := hugolib.LoadConfig(osFs, ".", "")
	if err != nil {
		return
	}

	cfg.Cfg = config

	fmt.Println("test")

	sites, err := hugolib.NewHugoSites(*cfg)
	if err != nil {
		return
	}
	_ = sites
}
